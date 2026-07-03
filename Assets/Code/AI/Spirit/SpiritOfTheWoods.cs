using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SpiritOfTheWoods : MonoBehaviour // Mort
{
    public static SpiritOfTheWoods instance;

    [HideInInspector] public GameObject spiritAnchor;
    
    public bool isActive;
    [SerializeField] float followSpeed = .6f;
    [SerializeField] float followdistance = 1f;

    private void Awake()
    {
        instance = this;
        HandleSpiritActivation(false);
    }

    private void LateUpdate()
    {
        if (Vector2.Distance(transform.position, spiritAnchor.transform.position) > followdistance)
        {
            transform.position = Vector2.Lerp(transform.position, spiritAnchor.transform.position, followSpeed * Time.deltaTime);
        }
    }

    public void HandleSpiritActivation(bool toggle)
    {
        isActive = toggle;

        if (isActive)
        {
            foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
            {
                particle.Play();
            }

            GetComponentInChildren<Light2D>().enabled = true;
        }
        else
        {
            foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
            {
                particle.Stop();
            }

            GetComponentInChildren<Light2D>().enabled = false;
        }
    }
}
