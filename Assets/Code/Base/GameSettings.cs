using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoulHunter
{
    public class GameSettings : MonoBehaviour // Mort
    {
        // Singleton Instance
        public static GameSettings Instance;

        // List of resolution options
        public List<string> options = new List<string>();
        public Resolution[] resolutions;

        // Saved Settings
        public static bool isFullscreen;
        public static bool tutorialsEnabled;
        public static int qualityIndex;
        public static int resolution;
        public static float soundVolume;
        public static float dialogueSpeed;

        // Audio Mixer
        public AudioMixer audioMixer;
        public AudioMixerGroup audioMixerGroup;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GetResolutions();
            SetDefaultSettings();
        }

        /// <summary>
        /// Prepares list of resolutions for use
        /// </summary>
        void GetResolutions()
        {
            resolutions = Screen.resolutions;
            System.Array.Reverse(resolutions);

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    resolution = i;
                }
            }
        }

        /// <summary>
        /// Sets the default settings to use at game start
        /// </summary>
        void SetDefaultSettings()
        {
            // If game starts at main menu
            if (SceneController.Instance.GetBuildIndex() == 0)
            {
                isFullscreen = true;
                tutorialsEnabled = true;
                qualityIndex = 3;
                soundVolume = 10;
                dialogueSpeed = 0f;
            }
            else
            {// else if game starts in a level (developer settings)
                isFullscreen = true;
                tutorialsEnabled = false;
                qualityIndex = 3;
                soundVolume = 10;
                dialogueSpeed = 0f;
            }
        }
    }
}