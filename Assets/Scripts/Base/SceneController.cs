using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class SceneController : MonoBehaviour // Mort
{
    // Singleton Instance
    public static SceneController Instance;

    LoadingScreen loadingScreen;

    int requestedIndex;

    private void Awake()
    {
        // Set Instance
        Instance = this;
        loadingScreen = GetComponent<LoadingScreen>();
    }

    /// <summary>
    /// Returns the build index of the currently active scene
    /// </summary>
    /// <returns></returns>
    public int GetBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    /// <summary>
    /// Returns the total amount of scenes currently in Build Index
    /// </summary>
    /// <returns></returns>
    public int GetTotalBuildIndex()
    {
        return SceneManager.sceneCountInBuildSettings;
    }

    /// <summary>
    /// Transitions to a specified scene with a loading screen
    /// </summary>
    /// <param name="index"></param>
    public void TransitionScene(int index)
    {
        requestedIndex = index;
        loadingScreen.UpdateAnimatorValues(true, index);
    }

    /// <summary>
    /// Loads the scene specified with an int (Scene Index)
    /// </summary>
    /// <param name="sceneIndex"></param>
    void ChangeScene()
    {
        StartCoroutine(LoadSceneAsync(requestedIndex));
    }

    public void ResetScene()
    {
        requestedIndex = GetBuildIndex();
        loadingScreen.UpdateAnimatorValues(true, requestedIndex);
    }

    /// <summary>
    /// Ends the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadSceneAsync(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        loadingScreen.ActivateElements();

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingScreen.UpdateValues(progress);
            yield return null;
        }

        loadingScreen.DisableElements();

        loadingScreen.UpdateAnimatorValues(false, -1);
    }
}