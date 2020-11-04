using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Aircraft
{
    public class AircraftPlayer : AircraftAgents
    {
        [Header("Input Bindings")]
        public InputAction pitchInput;
        public InputAction yawInput;
        public InputAction boostInput;
        public InputAction pauseInput;

        public override void Initialize()
        {
            base.Initialize();
            pitchInput.Enable();
            yawInput.Enable();
            boostInput.Enable();
            pauseInput.Enable();
        }
        /// <summary>
        /// Clean up
        /// </summary>
        private void OnDestroy()
        {
            pitchInput.Disable();
            yawInput.Disable();
            boostInput.Disable();
            pauseInput.Disable();
        }

        public override void Heuristic(float[] actionsOut)
        {
            //Pitch: 1= up ,0 = none, 2 = down
            float pitchValue = Mathf.Round(pitchInput.ReadValue<float>());
            float yawValue = Mathf.Round(yawInput.ReadValue<float>());
            float boostValue = Mathf.Round(boostInput.ReadValue<float>());
            //convert -1 (down) to discrete value 2
            if (pitchValue == -1f) pitchValue = 2f;
            if (yawValue == -1f) yawValue = 2f;
            actionsOut[0] = pitchValue;
            actionsOut[1] = yawValue;
            actionsOut[2] = boostValue;
            //foreach (object o in actionsOut) Debug.Log(o.ToString());
        }


    }
}