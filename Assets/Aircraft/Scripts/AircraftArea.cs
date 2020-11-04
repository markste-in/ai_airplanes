using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


namespace Aircraft{

    public class AircraftArea : MonoBehaviour
    {
        [Tooltip("The path the race will take")]
        public CinemachineSmoothPath racePath;

        [Tooltip("The prefab to use for checkpoints")]
        public GameObject checkpointPrefab;

        [Tooltip("Prefab to use for finish/start")]
        public GameObject finishPrefab;

        [Tooltip("If true enable training mode")]
        public bool trainingMode = false;

        public List<AircraftAgents> AircraftAgents {get; private set;}

        public List<GameObject> Checkpoints {get; private set;}

        //Actions to performwhen the airplane wakes up
        private void Awake()
        {
            if (AircraftAgents == null) FindAircraftAgents();
            if (Checkpoints == null) CreateCheckpoints();

        }

        private void Start()
        {
            


        }


        /// <summary>
        /// Find Aircraft Agents in the Area
        ///
        /// </summary>
        private void FindAircraftAgents()
        {
            AircraftAgents = transform.GetComponentsInChildren<AircraftAgents>().ToList();
            Debug.Assert(AircraftAgents.Count > 0, "No AircraftAgents in List");
            Debug.Log("Found " + AircraftAgents.Count.ToString() + " agents");
        }


        /// <summary>
        /// Creates the Checkpoints
        /// </summary>
        private void CreateCheckpoints()
        {
            //Create checkpoints along the path
            Debug.Assert(racePath != null, "Race Path was not set");
            Checkpoints = new List<GameObject>();
            int numCheckpoints = (int)racePath.MaxUnit(CinemachinePathBase.PositionUnits.PathUnits);
            for (int i = 0; i < numCheckpoints; i++)
            {
                GameObject checkpoint;
                if (i == numCheckpoints - 1)
                    checkpoint = Instantiate<GameObject>(finishPrefab);
                else
                    checkpoint = Instantiate<GameObject>(checkpointPrefab);
                //Set position, parent and rotation
                checkpoint.transform.SetParent(racePath.transform);
                checkpoint.transform.localPosition = racePath.m_Waypoints[i].position;
                checkpoint.transform.rotation = racePath.EvaluateOrientationAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);

                Checkpoints.Add(checkpoint);
            }
        }

        /// <summary>
        /// Resets the position of an agent using its current NextCheckpointIndex, unless randomize is true
        /// </summary>
        /// <param name="agent">The agent to reset</param>
        /// <param name="randomized">If true, will pick a new NextCheckpointIndex before reset</param>        
        public void ResetAgentPosition(AircraftAgents agent, bool randomized = true)
        {
            if (AircraftAgents == null) FindAircraftAgents();
            if (randomized)
            {
                agent.NextCheckpointIndex = Random.Range(0, Checkpoints.Count);

            }
            int previousCheckpointIndex = agent.NextCheckpointIndex - 1;
            if (previousCheckpointIndex < 0)
                previousCheckpointIndex = Checkpoints.Count - 1;

            float startPosition = racePath.FromPathNativeUnits(previousCheckpointIndex, CinemachinePathBase.PositionUnits.PathUnits);

            Vector3 basePosition = racePath.EvaluatePosition(startPosition);
            Quaternion orientation = racePath.EvaluateOrientation(startPosition);
            //Calculate a horizontal offset so the agents are spread apart
            Vector3 positionOffset = Vector3.right * (AircraftAgents.IndexOf(agent) - AircraftAgents.Count / 2f) * Random.Range(9f,10f);
            agent.transform.position = basePosition + orientation * positionOffset;
            agent.transform.rotation = orientation;
        }

    }

}