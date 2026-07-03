using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] GameObject sliderObject;
    [SerializeField] GameObject imageObject;
    [SerializeField] TextMeshProUGUI gameTips;
    [SerializeField] TextMeshProUGUI progressPercentage;

    private Slider slider;

    Animator transitionAnimator;

    [SerializeField] LoadingScreenLibrary library;

    private void Awake()
    {
        slider = sliderObject.GetComponent<Slider>();
        transitionAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        ShuffleContents();
    }

    public void UpdateValues(float progress)
    {
        slider.value = progress;
        progressPercentage.text = progress * 100 + "%";
    }

    /// <summary>
    /// Updates animator values. Give a negative number to sceneIndex if you do not wish to load a new scene.
    /// </summary>
    /// <param name="isLoading"></param>
    /// <param name="sceneIndex"></param>
    public void UpdateAnimatorValues(bool isLoading, int sceneIndex)
    {
        transitionAnimator?.SetBool("isLoading", isLoading);

        if (sceneIndex < 0)
        {
            transitionAnimator?.SetInteger("index", sceneIndex);
        }
    }

    public void ActivateElements()
    {
        imageObject.SetActive(true);
        sliderObject.SetActive(true);
    }

    public void DisableElements()
    {
        sliderObject.SetActive(false);
    }

    /// <summary>
    /// Shuffles loading screen image and tip
    /// </summary>
    public void ShuffleContents()
    {
        // If library of loading screen images is empty, return.
        if (library.artwork.Length > 0)
        {
            // If artwork object is null, return.
            if (!imageObject) return;

            // If artwork object has image component.
            if (imageObject.TryGetComponent(out Image artwork))
            {
                // Replace image with another in the library.
                artwork.sprite = library.artwork[Random.Range(0, library.artwork.Length)];
            }
            else
            {
                Debug.LogError("Loadingscreen image component does not exist.");
            }
            
        }

        // If library of loading screen tips is empty, return.
        if (library.tips.Length > 0)
        {
            // if tips text component is not null.
            if (gameTips)
            {
                // Replace tip with another in the library.
                gameTips.text = library.tips[Random.Range(0, library.tips.Length)].ToString();
            }
            else
            {
                Debug.LogError("Game tips text component is not assigned.");
            }
        }
    }
}

[System.Serializable]
public class LoadingScreenLibrary
{
    public Sprite[] artwork;
    [TextArea(3, 10)]
    public string[] tips;
}
