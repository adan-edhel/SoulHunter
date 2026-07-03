using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour // Mort
{
    void Start()
    {
        // Destroys particle after specified time
        Destroy(gameObject, 2f);
    }
}
