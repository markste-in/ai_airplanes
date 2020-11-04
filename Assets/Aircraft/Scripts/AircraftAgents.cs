using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

namespace Aircraft
{

    public class AircraftAgents : Agent
    {
        [Header("Movement Parameters")]
        public float thrust = 100000f;
        public float pitchSpeed = 100f;
        public float yawSpeed = 100f;
        public float rollSpeed = 100f;
        public float boostMultiplier = 2f;


        [Header("Explosioin Stuff")]
        public GameObject meshObject;

        [Header("The game object of the explosion particle effect")]
        public GameObject explosionEffect;

        [Header("Training")]
        [Tooltip("Number of steps to time out after in training")]
        public int stepTimeout = 500;

        public int NextCheckpointIndex { get; set; }

        //Components to keep track of
        private AircraftArea area;
        new private Rigidbody rigidbody;
        private TrailRenderer trail;

        //When the next step timeout will be during training
        private float nextStepTimeout;

        //Whether the aircraft is frozen (intentionally not flying)
        private bool froozen = false;

        //Control
        private float pitchChange = 0f;
        private float smoothPitchChange = 0f;
        private float maxPitchChange = 45f;
        private float yawChange = 0f;
        private float smoothYawChange = 0f;
        private float rollChange = 0f;
        private float smoothRollChange = 0f;
        private float maxRollAngle = 70f;
        private bool boost = false;


        /// <summary>
        /// called when agent is first initilized
        /// </summary>
        public override void Initialize()
        {
            area = GetComponentInParent<AircraftArea>();
            rigidbody = GetComponent<Rigidbody>();
            trail = GetComponent<TrailRenderer>();

            //Override the next step set in Inspector
            //Max 5000 step if training, infinite stefs if racing
            MaxStep = area.trainingMode ? 5000 : 0;
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            //Observe aircraft velocity
            sensor.AddObservation(transform.InverseTransformDirection(rigidbody.velocity));
            //Where is the next checkpoint
            sensor.AddObservation(VectorToNextCheckpoint());
            //Orientation of the next checkpoint
            sensor.AddObservation(transform.InverseTransformDirection(area.Checkpoints[NextCheckpointIndex].transform.forward));
            //Total Observation = 3+3+3 = 9
            
        }
        public override void Heuristic(float[] actionsOut)
        {
            Debug.LogError("Heurist was called on " + gameObject.name + " Make sure only Aircraft player call this function!");
        }

        /// <summary>
        /// Read action inputs from vectorActions
        /// </summary>
        /// <param name="vectorAction"></param>
        public override void OnActionReceived(float[] vectorAction)
        {
            if (froozen) return;

            //Read values for pitch and yaw
            pitchChange = vectorAction[0];
            if (pitchChange == 2f) pitchChange = -1f; //down

            yawChange = vectorAction[1];
            if (yawChange == 2f) yawChange = -1f; //action 2 = down

            boost = vectorAction[2] == 1;
            trail.emitting = boost;

            ProcessMovement();

            if (area.trainingMode)
            {
                //add a small negetive reward
                AddReward(-1f / MaxStep);

                //Make sure we haven't run out of time in training

                if (StepCount > nextStepTimeout)
                {
                    AddReward(-0.5f);
                    EndEpisode();
                }

                Vector3 localCheckpointDir = VectorToNextCheckpoint();

                if(localCheckpointDir.magnitude < Academy.Instance.EnvironmentParameters.GetWithDefault("checkpoint_radius",0f))
                {
                    GotCheckpoint();
                }

            }
        }


        /// <summary>
        /// Gets a Vector to the next Checkpoitn the agents needs to fly through
        /// </summary>
        /// <returns></returns>
        private Vector3 VectorToNextCheckpoint()
        {
            Vector3 nextCheckpointDir = area.Checkpoints[NextCheckpointIndex].transform.position - transform.position;
            Vector3 localCheckpointDir = transform.InverseTransformDirection(nextCheckpointDir);
            return localCheckpointDir;
        }

        private void GotCheckpoint()
        {
            NextCheckpointIndex = (NextCheckpointIndex + 1) % area.Checkpoints.Count;
            if (area.trainingMode)
            {
                AddReward(0.5f);
                nextStepTimeout = StepCount + stepTimeout; 
            }
        }
        public override void OnEpisodeBegin()
        {
            //Reset the velocity,position and orientation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            trail.emitting = false;
            area.ResetAgentPosition(agent: this, randomized: area.trainingMode);

            if (area.trainingMode) nextStepTimeout = StepCount + stepTimeout;
        }

        public void FreezeAgent()
        {
            Debug.Assert(area.trainingMode == false, "Freeze/Thaw not supported in training mode");
            froozen = true;
            rigidbody.Sleep();
           

        }

        public void ThawAgent()
        {
            Debug.Assert(area.trainingMode == false, "Freeze/Thaw not supported in training mode");
            froozen = false;
            rigidbody.WakeUp();
        }

        

        //Calculate and apply movement
        private void ProcessMovement()
        {
            //Calculate floot
            float boastModifier = boost ? boostMultiplier : 1f;

            //Apply forward thrust
            rigidbody.AddForce(transform.forward * thrust * boostMultiplier, ForceMode.Force);

            //Get current rotation
            Vector3 curAngle= transform.rotation.eulerAngles;

            //Calculate the roll angle (between -180 and 180)
            float rollAngle = curAngle.z > 180f ? curAngle.z - 360f : curAngle.z;

            if (yawChange==0f)
            {
                rollChange = -rollAngle / maxRollAngle;
            }
            else
            {
                rollChange = -yawChange;
            }

            smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
            smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);
            smoothRollChange = Mathf.MoveTowards(smoothRollChange, rollChange, 2f * Time.fixedDeltaTime);

            //Calculate new pitch, yaw and roll. Clamp pitch and roll
            float pitch = curAngle.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
            if (pitch > 180f) pitch -= 360f;
            pitch = Mathf.Clamp(pitch, -maxPitchChange, maxPitchChange);

            float yaw = curAngle.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

            float roll = curAngle.z + smoothRollChange * Time.fixedDeltaTime * rollSpeed;
            if (roll > 180f) roll -= 360f;
            roll = Mathf.Clamp(roll, -maxRollAngle, maxRollAngle);
            //Debug.Log("pitch: " + pitch + " yaw:" + yaw);
            //Debug.Log("pitch change: " + pitchChange + " yaw change:" +  yawChange);
            //Debug.Log("smooth pitch: " + smoothPitchChange + " smooth yaw:" + smoothYawChange);
            transform.rotation = Quaternion.Euler(pitch, yaw, roll);

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("checkpoint") && (other.gameObject == area.Checkpoints[NextCheckpointIndex]))
                GotCheckpoint();
        }
        /// <summary>
        /// react to collisions
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(this.gameObject.name + " collided with " + collision.gameObject.name + " " + collision.collider.tag);
            if (!collision.transform.CompareTag("agent"))
            {
                if(area.trainingMode)
                {
                    AddReward(-1f);
                    EndEpisode();
                    return;
                }
                else
                {
                    StartCoroutine(ExplosionReset());
                }
            }
            

        }
        //Reset the aircraft to the most recent complete checkpoint
        private IEnumerator ExplosionReset()
        {
            FreezeAgent();

            //Disable airplane mesh
            meshObject.SetActive(false);
            explosionEffect.SetActive(true);
            yield return new WaitForSeconds(2f);
            meshObject.SetActive(true);
            explosionEffect.SetActive(false);

            area.ResetAgentPosition(agent: this,area.trainingMode);
            yield return new WaitForSeconds(1f);

            ThawAgent();



        }

        


    }

}
