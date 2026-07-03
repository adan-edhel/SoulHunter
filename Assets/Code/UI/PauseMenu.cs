using UnityEngine.UI;
using UnityEngine;

using SoulHunter.Input;
using SoulHunter.Player;
using SoulHunter.Dialogue;

namespace SoulHunter.UI
{
    public class PauseMenu : MonoBehaviour, Input.ITogglePause // Mort
    {
        public GameObject PauseMenuUI;
        [SerializeField] Button continueButton;

        private void Start()
        {
            InputController.Instance.i_TogglePause = this;

            GameManager.gameIsPaused = false;
            HandlePause();
        }

        /// <summary>
        /// Toggles pause state
        /// </summary>
        public void TogglePause()
        {
            GameManager.gameIsPaused = !GameManager.gameIsPaused;
            HandlePause();
        }

        // Handles game pause
        void HandlePause()
        {
            if (!GameManager.gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
            DialogueManager.Instance.PauseCheck();
        }

        /// <summary>
        /// Resume game
        /// </summary>
        void Resume()
        {
            if (!DialogueManager.inDialogue)
            {
                PlayerBase.isPaused = false;
            }

            PauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameManager.gameIsPaused = false;
        }

        /// <summary>
        /// Pause game
        /// </summary>
        void Pause()
        {
            PlayerBase.isPaused = true;

            PauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameManager.gameIsPaused = true;

            continueButton.Select();
        }

        /// <summary>
        /// Change scene function for buttons
        /// </summary>
        /// <param name="index"></param>
        public void ChangeScene(int index)
        {
            SceneController.Instance.TransitionScene(index);
        }
    }
}