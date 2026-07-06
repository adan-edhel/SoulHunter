using UnityEngine;

namespace SoulHunter.UI
{
    public class CanvasHandler : MonoBehaviour // Mort - Allows the UI to be accessed with a singleton
    {
        // Singleton instance
        public static CanvasHandler Instance;

        // Menu UI variable
        public GameObject MenuCanvas { get; set; }

        // Game UI variable
        public GameObject GameCanvas { get; set; }

        private void Awake()
        {
            Instance = this;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}