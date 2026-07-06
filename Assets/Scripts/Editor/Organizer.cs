using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;

public class Organizer : EditorWindow // Mort
{
    GUISkin mySkin;

    static string deletedFolders;

    static string ArtDir = "Assets/Art/";
    static string DevDir = "Assets/Code/";
    static string AudioDir = "Assets/Audio/";

    [MenuItem("Tools/Organizer", false, 1)]
    public static void Open()
    {
        Organizer m_organizer = (Organizer)GetWindow(typeof(Organizer));
        m_organizer.titleContent = new GUIContent("Organizer");
        m_organizer.Show();
    }

    private void OnEnable()
    {
        mySkin = Resources.Load("GUISkins/OrganizerSkin") as GUISkin;
    }

    private void OnGUI()
    {
        GUI.skin = mySkin;
        GUILayout.Label("Create Folders");

        GUILayout.BeginHorizontal(); // Start Horizontal
        // Create folders for a 2D Project
        if (GUILayout.Button("2D Project Artist"))
        {
            CreateFolders2DArt();
            EditorUtility.DisplayDialog("Creating folders..", "Folders created.", "Okay");
        }
        // Create folders for a 3D Project
        if (GUILayout.Button("3D Project Artist"))
        {
            CreateFolders3DArt();
            EditorUtility.DisplayDialog("Creating folders..", "Folders created.", "Okay");
        }
        GUILayout.EndHorizontal(); // End Horizontal
        if (GUILayout.Button("Programmer"))
        {
            CreateCodeFolders();
            EditorUtility.DisplayDialog("Creating folders..", "Folders created.", "Okay");
        }
        if (GUILayout.Button("Audio"))
        {
            CreateAudioFolders();
            EditorUtility.DisplayDialog("Creating folders..", "Folders created.", "Okay");
        }

        GUILayout.Label("Clean Up");

        // Clean up empty folders
        if (GUILayout.Button("Delete All Empty Folders"))
        {
            Cleanup();
            EditorUtility.DisplayDialog("Cleaning up...", "Deleted all empty folders", "Okay");
        }

        GUILayout.FlexibleSpace();
        GUILayout.Label("Made by Mort");
    }

    /// <summary>
    /// Create necessary folders for 2D art
    /// </summary>
    static void CreateFolders2DArt()
    {
        //Game Sprites
        Directory.CreateDirectory(ArtDir + "Sprites/Player");
        Directory.CreateDirectory(ArtDir + "Sprites/Background");
        Directory.CreateDirectory(ArtDir + "Sprites/Environment");

        //UI Sprites
        Directory.CreateDirectory(ArtDir + "Sprites/Menu UI");
        Directory.CreateDirectory(ArtDir + "Sprites/Game UI");

        //Animations
        Directory.CreateDirectory(ArtDir + "Animations/Player");
        Directory.CreateDirectory(ArtDir + "Animations/Environment");

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Create necessary folders for 3D art
    /// </summary>
    static void CreateFolders3DArt()
    {
        //Models
        Directory.CreateDirectory(ArtDir + "Assets/Models/Player");
        Directory.CreateDirectory(ArtDir + "Assets/Models/Environment");

        //UI Sprites
        Directory.CreateDirectory(ArtDir + "Assets/Sprites/Menu UI");
        Directory.CreateDirectory(ArtDir + "Assets/Sprites/Game UI");

        //Textures
        Directory.CreateDirectory(ArtDir + "Assets/Textures/Player");
        Directory.CreateDirectory(ArtDir + "Assets/Textures/Environment");

        //Materials
        Directory.CreateDirectory(ArtDir + "Assets/Materials");

        //Animations
        Directory.CreateDirectory(ArtDir + "Assets/Animations/Player");
        Directory.CreateDirectory(ArtDir + "Assets/Animations/Environment");

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Create necessary folders for developers
    /// </summary>
    static void CreateCodeFolders()
    {
        Directory.CreateDirectory(DevDir + "Compilations");
        Directory.CreateDirectory(DevDir + "Editor");
        Directory.CreateDirectory(DevDir + "Input");
        Directory.CreateDirectory(DevDir + "Base");

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Create necessary folders for audio files
    /// </summary>
    static void CreateAudioFolders()
    {
        Directory.CreateDirectory(AudioDir + "Sound FX");
        Directory.CreateDirectory(AudioDir + "Music/Title");
        Directory.CreateDirectory(AudioDir + "Music/Game");

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    static string ScanDirectory(DirectoryInfo subDirectory)
    {
        var filesInSubDirectory = subDirectory.GetFiles("*.*", SearchOption.AllDirectories);

        if (filesInSubDirectory.Length == 0 || !filesInSubDirectory.Any(t => t.FullName.EndsWith(".meta") == false))
        {
            deletedFolders += subDirectory.FullName + "/n";
            subDirectory.Delete(true);
        }
        return deletedFolders;
    }

    static void Cleanup()
    {
        deletedFolders = string.Empty;

        var directoryInfo = new DirectoryInfo(Application.dataPath);
        foreach (var subDirectory in directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories))
        {
            if (subDirectory.Exists)
            {
                ScanDirectory(subDirectory);
            }
        }
        AssetDatabase.Refresh();
    }
}
