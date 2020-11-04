using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Aircraft
{

    public class MainMenuController : MonoBehaviour
    {
        public List<string> Levels;

        [Tooltip("Dropdown for selecting the level")]
        public TMP_Dropdown levelDropDown;

        [Tooltip("Dropdown for selecting game difficulty")]
        public TMP_Dropdown difficultyDropDown;

        private GameDifficulty selectedDifficutly;
        private string selectedLevel;
        /// <summary>
        /// Automatically fill dropdown list
        /// </summary>
        private void Start()
        {
            Debug.Assert(Levels.Count > 0, "No Levels available");
            levelDropDown.ClearOptions();
            levelDropDown.AddOptions(Levels);
            selectedLevel = Levels[0];

            difficultyDropDown.ClearOptions();
            difficultyDropDown.AddOptions(Enum.GetNames(typeof(GameDifficulty)).ToList());
            selectedDifficutly = GameDifficulty.HardDesert;
        }
        public void SetLevel(int levelIndex)
        {
            selectedLevel = Levels[levelIndex];
        }
        public void SetDifficulty(int DifficultyIndex)
        {
            selectedDifficutly = (GameDifficulty)DifficultyIndex;
        }
        public void StartButtonClicked()
        {
            // Set game difficulty
            GameManager.Instance.GameDifficulty = selectedDifficutly;
            //Load Level
            GameManager.Instance.LoadLevel(selectedLevel, GameState.Preparing);
        }

        public void QuitButtonClicked()
        {
            Application.Quit();
        }
    }
}