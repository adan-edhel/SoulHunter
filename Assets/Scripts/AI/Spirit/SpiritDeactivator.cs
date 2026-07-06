using SoulHunter.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
