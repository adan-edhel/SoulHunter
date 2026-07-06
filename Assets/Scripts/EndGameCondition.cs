using SoulHunter.Dialogue;
using UnityEngine;

public class EndGameCondition : MonoBehaviour
{
    private void OnDestroy()
    {
        SceneController.Instance.TransitionScene(0);
    }
}
