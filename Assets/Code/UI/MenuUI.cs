using UnityEngine;

namespace SoulHunter.UI
{
    public class MenuUI : MonoBehaviour // Mort
    {
        void Start()
        {
            // Sets reference inside UIManager
            CanvasHandler.Instance.MenuCanvas = gameObject;
            // Shows cursor
            Cursor.visible = true;
        }

        /// <summary>
        /// Change scene function for buttons
        /// </summary>
        /// <param name="index"></param>
        public void ChangeScene(int index)
        {
            SceneController.Instance.TransitionScene(index);
        }

        /// <summary>
        /// Quit game function for buttons
        /// </summary>
        public void QuitGame()
        {
            SceneController.Instance.QuitGame();
        }
    }
}