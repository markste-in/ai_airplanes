using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Aircraft
{

    public class GameOverUIController : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI placeText;

        private RaceManager raceManager;

        private void Awake()
        {
            raceManager = FindObjectOfType<RaceManager>();

        }

        private void OnEnable()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.GameState == GameState.Gameover)
            {
                string place = raceManager.GetAgentPlace(raceManager.FollowAgent);
                placeText.text = place + " Place";
            }
        }

        public void MainMenuBottonClicked()
        {
            GameManager.Instance.LoadLevel("MainMenu", GameState.MainMenu);
        }
    }
}
