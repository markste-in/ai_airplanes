using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aircraft
{

    public class PauseController : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.OnStateChange += OnStateChange;
        }

        private void OnStateChange()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
                gameObject.SetActive(false);
        }
        public void MainMenuButtonClicked()
        {
            GameManager.Instance.LoadLevel("MainMenu", GameState.MainMenu);
        }
        public void ResumeButtonClicked()
        {
            GameManager.Instance.GameState = GameState.Playing;
        }
        private void OnDestroy()
        {
            if (GameManager.Instance != null) GameManager.Instance.OnStateChange -= OnStateChange;
        }
    }

}