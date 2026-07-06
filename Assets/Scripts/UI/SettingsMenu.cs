using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace SoulHunter.UI
{
    public class SettingsMenu : MonoBehaviour // Mort
    {
        [SerializeField] Toggle fullscreenToggle;
        [SerializeField] Toggle tutorialsToggle;
        [SerializeField] TMP_Dropdown graphicsDropdown;
        [SerializeField] TMP_Dropdown resolutionsDropdown;

        [SerializeField] Slider volumeSlider;
        [SerializeField] TextMeshProUGUI volumePercentage;
        [SerializeField] Slider dialogueSlider;
        [SerializeField] TextMeshProUGUI dialoguePercentage;

        private void Start()
        {
            SetUpResolutions();
            UpdateValues();
        }

        /// <summary>
        /// Fills dropdown menu with resolutions and sets up current resolution
        /// </summary>
        void SetUpResolutions()
        {
            resolutionsDropdown.ClearOptions();
            resolutionsDropdown.AddOptions(GameSettings.Instance.options);
            resolutionsDropdown.value = GameSettings.resolution;
            resolutionsDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Updates settings menu with saved game settings
        /// </summary>
        void UpdateValues()
        {
            graphicsDropdown.value = GameSettings.qualityIndex;

            if (GameSettings.isFullscreen)
            {
                fullscreenToggle.isOn = true;
                Application.runInBackground = false;
            }
            else
            {
                fullscreenToggle.isOn = false;
                Application.runInBackground = true;
            }

            tutorialsToggle.isOn = GameSettings.tutorialsEnabled;

            volumeSlider.value = GameSettings.soundVolume / 10;
            SetVolume(GameSettings.soundVolume / 10);

            dialogueSlider.value = 1.0f - GameSettings.dialogueSpeed;
        }

        /// <summary>
        /// Toggles application fullscreen
        /// </summary>
        /// <param name="fullscreen"></param>
        public void ToggleFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Application.runInBackground = false;
                GameSettings.isFullscreen = true;
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Application.runInBackground = true;
                GameSettings.isFullscreen = false;
            }
        }

        /// <summary>
        /// Toggles in-game tutorials
        /// </summary>
        /// <param name="toggle"></param>
        public void ToggleTutorials(bool toggle)
        {
            GameSettings.tutorialsEnabled = toggle;
        }

        /// <summary>
        /// Sets texture quality in settings
        /// </summary>
        /// <param name="qualityIndex"></param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            GameSettings.qualityIndex = qualityIndex;
        }

        /// <summary>
        /// Sets resolution in settings
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = GameSettings.Instance.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            GameSettings.resolution = resolutionIndex;
        }

        /// <summary>
        /// Sets game volume in settings
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {
            GameSettings.Instance?.audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
            GameSettings.soundVolume = volume * 10;

            float displayValue = Mathf.Round(GameSettings.soundVolume * 10);
            volumePercentage.text = displayValue.ToString();
        }

        /// <summary>
        /// Sets dialogue speed in settings
        /// </summary>
        /// <param name="speed"></param>
        public void SetDialogueSpeed(float speed)
        {
            GameSettings.dialogueSpeed = 1.0f - speed;
            dialoguePercentage.text = Mathf.Round(speed * 10).ToString();
        }
    }
}