using Cinemachine;
using SoulHunter;
using SoulHunter.Dialogue;
using UnityEngine;

public class CameraManager : MonoBehaviour // Mort
{
    public static CameraManager Instance;

    // Cinemachine Shake Values
    [SerializeField] float defaultShakeAmplitude = 1.2f;         // Cinemachine Noise Profile Parameter
    [SerializeField] float defaultShakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter

    float ShakeElapsedTime = 0f;

    // Cinemachine Virtual Camera
    private CinemachineVirtualCamera virtualCamera;

    // Cinemachine Components
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    private CinemachineFollowZoom virtualCameraFollowZoom;
    private CinemachineTargetGroup targetGroup;
    private CinemachineConfiner confiner;


    private void Awake()
    {
        Instance = this;

        // Get virtual camera & components
        virtualCamera =  GetComponentInChildren<CinemachineVirtualCamera>();
        targetGroup = GetComponentInChildren<CinemachineTargetGroup>();
        confiner = virtualCamera.GetComponent<CinemachineConfiner>();
    }

    private void Start()
    {
        // If virtual camera exists, get camera noise component
        if (virtualCamera != null)
        {
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
            virtualCameraFollowZoom = virtualCamera.GetComponent<CinemachineFollowZoom>();
        }
    }

    /// <summary>
    /// Update confiner on the virtual camera
    /// </summary>
    /// <param name="collider"></param>
    public void UpdateConfiner(PolygonCollider2D collider)
    {
        confiner.m_BoundingShape2D = collider;
        confiner.InvalidatePathCache();
    }

    /// <summary>
    /// Shakes the camera with the following parameters: Duration, Amplitude, Frequency. -- Duration always needs to be given a value, amplitude and frequency will use default value when left at 0. Defaults: Amp(1.2f), Freq(2.0f).
    /// </summary>
    public void ShakeCamera(float duration, float amplitude, float frequency)
    {
        ShakeElapsedTime = duration / 10;

        // Set Cinemachine Camera Noise parameters
        if (amplitude <= 0)
        {
            virtualCameraNoise.m_AmplitudeGain = defaultShakeAmplitude;
        }
        else
        {
            virtualCameraNoise.m_AmplitudeGain = amplitude;
        }
        if (frequency <= 0)
        {
            virtualCameraNoise.m_FrequencyGain = defaultShakeFrequency;
        }
        else
        {
            virtualCameraNoise.m_FrequencyGain = frequency;
        }
    }

    private void Update()
    {
        // If the virtual camera is null, avoid update.
        if (!virtualCamera) return;

        // If required cinemachine components are null, avoid update.
        if (!virtualCameraNoise || !virtualCameraFollowZoom) return;

        //----------------------------------------

        // Set Cinemachine Camera Noise parameters
        if (ShakeElapsedTime > 0)
        {
            // Update Shake Timer
            ShakeElapsedTime -= Time.deltaTime;
        }
        else
        {
            // If Camera Shake effect is over, reset variables
            virtualCameraNoise.m_AmplitudeGain = 0f;
            ShakeElapsedTime = 0f;
        }

        // If player is in a dialogue, zoom in
        if (DialogueManager.inDialogue && virtualCameraFollowZoom.m_Width > 1)
        {
            virtualCameraFollowZoom.m_Width -= Time.deltaTime * 10;
            targetGroup.m_Targets[1].radius = 0;
        }
        // If player is no longer in dialogue, zoom out
        else if (!DialogueManager.inDialogue && virtualCameraFollowZoom.m_Width < 12)
        {
            virtualCameraFollowZoom.m_Width += Time.deltaTime * 10;
            targetGroup.m_Targets[1].radius = 4;
        }
    }
}
