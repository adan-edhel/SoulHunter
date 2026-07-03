using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointObject : MonoBehaviour
{
    void Start()
    {
        WayPointScript AI = FindObjectOfType<WayPointScript>();
        AI.waypoints.Enqueue(gameObject);
    }

    void Update()
    {

    }
}