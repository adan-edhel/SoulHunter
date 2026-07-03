using System.IO;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    // Name and location of the Manager prefab
    static string ManagerPath = "Base/Managers";

    /// <summary>
    /// Loads the necessary prefabs before the scene is loaded
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        Instantiate(Resources.Load(ManagerPath));
    }
}