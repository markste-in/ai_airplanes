using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aircraft
{
    public enum GameState
    {
        Default,
        MainMenu,
        Preparing,
        Playing,
        Paused,
        Gameover
    }

    public enum GameDifficulty
    {
        Normal,
        HardDesert,
        HardSnow
    }

    public delegate void OnStateChangeHandler();


    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Event is called when game state changes
        /// </summary>
        public event OnStateChangeHandler OnStateChange;
        private GameState gameState;

        public GameState GameState
        {
            get {
                return gameState;
            }
            set
            {
                gameState = value;
                if (OnStateChange != null) OnStateChange();
            }
        }

        public GameDifficulty GameDifficulty { get; set; }
        /// <summary>
        /// The singleton GameManger instance
        /// </summary>
        public static GameManager Instance
        {
            get; private set;
        }

        /// <summary>
        /// Manages the singleton and set fullscreen resolution
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                //Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,true);
                this.GameDifficulty = GameDifficulty.HardDesert;
            }
            else
                Destroy(gameObject);
        }

        public void OnApplicationQuit()
        {
            Instance = null;
        }
        /// <summary>gigit
        /// loads a new level and set game state
        /// </summary>
        /// <param name="LevelName"></param>
        /// <param name="newState"></param>
        public void LoadLevel(string LevelName, GameState newState)
        {
            StartCoroutine(LoadLevelAsync(LevelName, newState));
        }

        public IEnumerator LoadLevelAsync(string LevelName, GameState newState)
        {
            //Load new level
            AsyncOperation operation = SceneManager.LoadSceneAsync(LevelName);
            while (operation.isDone == false)
                yield return null;
            //Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            GameState = newState;
        }



    }
}
