using System.Collections.Generic;
using SoulHunter.Player;
using UnityEngine;
using SoulHunter;

public class PortalScript : Interactable // Mort
{
    // Scriptable Object
    [SerializeField] PortalData portalType;

    // Closest Cinemachine Confiner
    PolygonCollider2D closestConfiner;

    // List of connected portals
    List<PortalScript> connectedPortals = new List<PortalScript>();

    // Layers to check collision with
    [SerializeField] LayerMask aliveEntities;

    private void Awake()
    {
        GameManager.Instance.PortalListRegistry(this);
    }

    private void Start()
    {
        // Assign portals of the same type to Connected Portals list
        AssignConnectedPortals();

        // Assign closest Cinemachine Virtualcam Confiner
        closestConfiner = AssignClosestConfiner();

        // Stop all particles
        foreach (var particleSystem in GetComponentsInChildren<ParticleSystem>())
        {
            particleSystem.Stop();
        }
    }

    PolygonCollider2D AssignClosestConfiner()
    {
        // Save all objects with "Confiner" tag in an array
        GameObject[] allConfiners = GameObject.FindGameObjectsWithTag("Confiner");
        float shortestDistance = Vector3.Distance(allConfiners[0].transform.position, transform.position);
        GameObject nearestConfiner = allConfiners[0];

        // Loop through all confiner objects and assign the closest one to a variable
        foreach (var confiner in allConfiners)
        {
            var pos = confiner.transform.position;
            var dist = Vector3.Distance(pos, transform.position);

            if (dist <= shortestDistance)
            {
                nearestConfiner = confiner;
            }
        }
        
        // Return the collider of the nearest confiner
        return nearestConfiner.GetComponent<PolygonCollider2D>();
    }

    void AssignConnectedPortals()
    {
        // Save Portals that have the same assigned color into Connected Portals list
        for (int i = 0; i < GameManager.Instance.Portals.Count; i++)
        {
            if (GameManager.Instance.Portals[i].Equals(this))
            {
                continue;
            }

            if (GameManager.Instance.Portals[i].portalType.color == portalType.color)
            {
                connectedPortals.Add(GameManager.Instance.Portals[i]);
            }
        }

        // Warn if no portals are connected to current portal
        if (connectedPortals.Count == 0)
        {
            isActivatable = false;
            Debug.LogError($"No portals connected to {gameObject.name} in {transform.root.name}.");
        }

        // Warn if multiple portals are connected to current portal
        if (connectedPortals.Count > 1)
        {
            Debug.Log($"Multiple portals connected to {gameObject.name} in {transform.root.name}. This will randomize its destination.");
        }
    }

    /// <summary>
    /// Selects a connected portal to send the player to
    /// </summary>
    /// <returns></returns>
    Transform SelectPortalToTeleport()
    {
        return connectedPortals[Random.Range(0, connectedPortals.Count)].transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If collision is not an alive entity, return.
        if (!aliveEntities.Includes(collision.transform.gameObject.layer)) return; // Mort - custom extension method!

        if (isActivatable || isRepeatable)
        {
            // Play all particles
            foreach (var particleSystem in GetComponentsInChildren<ParticleSystem>())
            {
                particleSystem.Play();
            }
        }

        // If collision is on player layer
        if (collision.transform.gameObject.layer == 10)
        {   // Update Cinemachine Confiner
            CameraManager.Instance.UpdateConfiner(closestConfiner);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // If the portal is not activatable nor repeatable, return.
        if (!isActivatable && !isRepeatable) return;

        if (!GameManager.interacting) return;

        if (PlayerBase.isSwinging) return;

        // If colliding object is on player layer
        if (collision.transform.gameObject.layer == 10)
        {
            // Set player teleportation state
            PlayerBase.isTeleporting = true;
            PlayerBase.isPaused = true;

            // Set player teleportation destination
            PlayerBase.teleportDestination = SelectPortalToTeleport();

            // Play teleportation audio
            AudioManager.PlaySound(AudioManager.Sound.TeleportDissolve, transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // If there are any alive entities inside, return.
        if (Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius, aliveEntities))
        {
            return;
        }

        // Stop all particles
        foreach (var particleSystem in GetComponentsInChildren<ParticleSystem>())
        {
            particleSystem.Stop();
        }
    }

    private void OnDestroy()
    {
        // Remove this from the list of portals once destroyed
        GameManager.Instance.PortalListRegistry(this);
    }
}