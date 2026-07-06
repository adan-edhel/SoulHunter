using SoulHunter;
using SoulHunter.Dialogue;
using SoulHunter.Player;
using UnityEngine;

public class SpiritActivator : MonoBehaviour
{
    DialogueTrigger dialogue;

    void Start()
    {
        dialogue = GetComponent<DialogueTrigger>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // If dialogue is already initiated, return.
        if (DialogueManager.inDialogue) return;

        // If collision is on the player layer
        if (collision.transform.gameObject.layer == 10)
        {
            // If player isn't interacting and dialogue can't be triggered by collision, return.
            if (!GameManager.interacting && !dialogue.canTriggerByCollision) return;

            // If player is swinging, return.
            if (PlayerBase.isSwinging) return;

            // If either activatable or repeatable, start dialogue.
            if (dialogue.isActivatable || dialogue.isRepeatable)
            {
                SpiritOfTheWoods.instance.HandleSpiritActivation(true);
            }
        }
    }
}
