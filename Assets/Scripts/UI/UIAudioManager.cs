using UnityEngine;

namespace SoulHunter.UI
{
    public class UIAudioManager : MonoBehaviour // Mort - simple script for UI audio
    {
        public AudioSource audioSource;

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

