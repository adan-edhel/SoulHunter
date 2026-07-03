using System.Collections.Generic;
using UnityEngine;

using SoulHunter;

public static class AudioManager // Mort - Followed Tutorial by Code Monkey, modified/adjusted to suit project
{
    public enum Sound
    {
        PlayerWalkGrass,    // Done
        PlayerWalkWood,     // Done
        PlayerJump,         // Done
        PlayerLandGrass,    // Done
        PlayerLandWood,     // Done
        PlayerAttackMiss,   // Done
        PlayerAttackHit,    // Done
        PlayerDeath,        // Done - subject to change
        GrappleThrow,       // Done
        GrappleDetach,      // Done
        TeleportDissolve,   // Done
        TeleportAppear,     // Done
        CollectSoul,        // Done
        EnemyDeath,         // Done
        StartDialogue,      // Done
        EndDialogue,        // Done
        ClothFlowing,       // Done
        AmbientBirds,       // Done - TODO: spawn at random spots in the level
        EnemyAttack,        // Done
        EnemyHurt,          // Done
        PlayerHurt
    }

    // Dictionary for intervals for some sound types
    private static Dictionary<Sound, float> soundTimerDictionary;

    /// <summary>
    /// Initializes audio timer dictionary values
    /// </summary>
    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerWalkGrass] = 0;
        soundTimerDictionary[Sound.PlayerWalkWood] = 0;
        soundTimerDictionary[Sound.PlayerJump] = 0;
        soundTimerDictionary[Sound.TeleportDissolve] = 0;
        soundTimerDictionary[Sound.TeleportAppear] = 0;
        soundTimerDictionary[Sound.ClothFlowing] = 0;
        soundTimerDictionary[Sound.StartDialogue] = 0;
    }

    /// <summary>
    /// Plays sound in 3D space
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="position"></param>
    public static void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.Play();

            audioSource.outputAudioMixerGroup = GameSettings.Instance.audioMixerGroup;
            if (audioSource.clip != null)
            {
                Object.Destroy(soundGameObject, GetAudioClip(sound).length);
            }
            else
            {
                Object.Destroy(soundGameObject);
            }
        }
    }

    /// <summary>
    /// Plays sound in 2D space
    /// </summary>
    /// <param name="sound"></param>
    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(GetAudioClip(sound));

            audioSource.outputAudioMixerGroup = GameSettings.Instance.audioMixerGroup;
            if (audioSource.clip != null)
            {
                Object.Destroy(soundGameObject, GetAudioClip(sound).length);
            }
            else
            {
                Object.Destroy(soundGameObject);
            }
        }
    }

    /// <summary>
    /// Sets timers for some sound types to play them in intervals
    /// </summary>
    /// <param name="sound"></param>
    /// <returns></returns>
    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.PlayerWalkGrass:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .45f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case Sound.PlayerWalkWood:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .45f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case Sound.PlayerJump:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .45f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case Sound.TeleportDissolve:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 1f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case Sound.TeleportAppear:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 1f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case Sound.ClothFlowing:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .3f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case Sound.StartDialogue:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .1f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
        }
        return false;
    }

    /// <summary>
    /// Gets a random audio clip from the array of each respective sound in the library as required
    /// </summary>
    /// <param name="sound"></param>
    /// <returns></returns>
    private static AudioClip GetAudioClip(Sound sound)
    {
        // Go through all sound types in the game
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClipArray)
        {
            // If the sound type matches the currently required sound
            if (soundAudioClip.sound == sound)
            {
                // If the audio clip array for the current sound type is not empty
                if (soundAudioClip.audioClip.Length > 0)
                {
                    // Pick a random audio clip inside the array of the current sound type
                    int randomClip = Random.Range(0, soundAudioClip.audioClip.Length);
                    // Return an error if the currently selected audio clip is null
                    if (soundAudioClip.audioClip[randomClip] == null)
                    {
                        Debug.LogError("Sound " + sound + " is not assigned a valid clip");
                    }
                    // Return the currently selected audio clip
                    return soundAudioClip.audioClip[randomClip];
                }
                else
                { // Return error if the audio clip array for the current sound type is empty
                    Debug.LogError("Sound " + sound + " array is empty!");
                    return null;
                }
            }
        }
        // Return error if the currently required sound type doesn't exist
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}
