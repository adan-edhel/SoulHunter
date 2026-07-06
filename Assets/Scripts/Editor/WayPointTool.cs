using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WayPointTool : EditorWindow
{
    string objectBaseName = "";
    int objectID = 1;
    GameObject Player;
    GameObject objectToSpawn;
    GameObject ParentObject;
    float spawnRadius = 5f;
    float Speed;
    int Amount;
    float MinDistance;

    bool playerSet = false;

    WayPointScript waypointScript;

    [MenuItem ("Tools/WayPointTool")]

    public static void ShowWindow()
    {
        GetWindow(typeof(WayPointTool));
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Player", EditorStyles.boldLabel);
        Player = (GameObject)EditorGUILayout.ObjectField(Player, typeof(GameObject), true);
        if (GUILayout.Button("Add Script"))
        {
            if (Player.GetComponent<WayPointScript>() == null)
            {
                waypointScript = Player.AddComponent<WayPointScript>();
                playerSet = true;
            }
        }
        GUILayout.Label("Add WayPoint", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Waypoint"))
        {
            if (ParentObject == null)
            {
                ParentObject = new GameObject("WayPoints");
            }
            objectToSpawn = new GameObject("WayPoint " + objectID);
            objectToSpawn.transform.parent = ParentObject.transform;
            if (Player != null)
            {
                objectToSpawn.AddComponent<WayPointObject>();
                Player.GetComponent<WayPointScript>().waypoints.Enqueue(objectToSpawn);

            }
            objectID++;
        }
        GUILayout.Label("Player Settings", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        Speed = EditorGUILayout.FloatField(Speed);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Amount of WayPoints");
        Amount = EditorGUILayout.IntField(Amount);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Minimum Distance");
        MinDistance = EditorGUILayout.FloatField(MinDistance);
        GUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(!playerSet);

        if (GUILayout.Button("Apply Settings"))
        {
            waypointScript.speed = Speed;
            waypointScript.num = Amount;
            waypointScript.minDist = MinDistance;
            EditorUtility.DisplayDialog("Success", "Succesfully added waypoints to the GameObject", "OK");
        }
        EditorGUI.EndDisabledGroup();
    }
}
