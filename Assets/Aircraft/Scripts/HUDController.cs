using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aircraft
{

    public class HUDController : MonoBehaviour
    {
        [Tooltip("The place in the race")]
        public TMP_Text placeText;
        [Tooltip("The remaining time to reach the next checkpoint")]
        public TMP_Text timeText;
        [Tooltip("The lap in the race")]
        public TMP_Text laptext;

        public Image checkpointIcon;
        public Image checkpointArrow;

        public float indicatorLimit = 0.7f;

        public AircraftAgents FollowAgent { get;  set; }

        private RaceManager raceManager;

        // Start is called before the first frame update
        private void Awake()
        {
            raceManager = FindObjectOfType<RaceManager>();
        }
        private void Update()
        {
            if (FollowAgent != null)
            {
                UpdacePlaceText();
                UpdateTimeText();
                UpdateLapText();
                UpdateArrow();
            }
            else
                Debug.Log("Follow Agent eq null");

        }

        private void UpdacePlaceText()
        {
            string place = raceManager.GetAgentPlace(FollowAgent);
            placeText.text = place;
        }

        private void UpdateTimeText()
        {
            float time = raceManager.GetAgentTime(FollowAgent);
            timeText.text = time.ToString("0.0");
        }

        private void UpdateLapText()
        {
            int lap = raceManager.GetAgentLap(FollowAgent);
            laptext.text = lap.ToString();
        }

        private void UpdateArrow()
        {
            // find the checkpoint withing the viewport
            Transform nextCheckpoint = raceManager.GetAgentNextCheckpoint(FollowAgent);
            Vector3 viewPoint = raceManager.ActiveCamera.WorldToViewportPoint(nextCheckpoint.transform.position);
            bool behindCamera = viewPoint.z < 0;
            viewPoint.z = 0f;
            //do position calculations
            Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0f);
            Vector3 fromCenter = viewPoint - viewportCenter;
            float HalfLimit = indicatorLimit / 2f;
            bool showArrow = false;

            if (behindCamera)
            {
                //Lmit distance from center
                //Viewport point is flipped when object is behind camera
                fromCenter = -fromCenter.normalized * HalfLimit;
                showArrow = true;
                //Debug.Log("Checkpoint behind camera");
            }
            else
            {
                //Debug.Log("Checkpoint ahead");
                if (fromCenter.magnitude > HalfLimit)
                {
                    fromCenter = fromCenter.normalized * HalfLimit;
                    showArrow = true;
                }
            }
            checkpointArrow.gameObject.SetActive(showArrow);
            checkpointArrow.rectTransform.rotation = Quaternion.FromToRotation(Vector3.up, fromCenter);
            checkpointIcon.rectTransform.position = raceManager.ActiveCamera.ViewportToScreenPoint(fromCenter + viewportCenter);
           //update the checkpoint arrow
        }
    }
}
