using UnityEngine;

namespace SoulHunter
{
    public class SpiritDeactivator : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (DataManager.Instance.soulsCollected < 14) return;

            // If collision is on the player layer
            if (collision.transform.gameObject.layer == 10)
            {
                SpiritOfTheWoods.instance.HandleSpiritActivation(false);
            }
        }
    }
}