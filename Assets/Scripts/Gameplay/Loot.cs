using UnityEngine;

using SoulHunter.Base;
using SoulHunter.Player;

public class Loot : MonoBehaviour
{
    [HideInInspector]
    public int soulPower;
    bool colliding;

    [SerializeField] GameObject particle;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (colliding)
        {
            return;
        }

        if (collision.transform.CompareTag("Player"))
        {
            colliding = true;
            DataManager.Instance.soulsCollected += soulPower;
            Instantiate(particle, new Vector2(transform.position.x, transform.position.y + 1), Quaternion.identity);
            AudioManager.PlaySound(AudioManager.Sound.CollectSoul, transform.position);
            collision.transform.GetComponent<PlayerBase>().Heal();
            Destroy(gameObject);
            colliding = false;
        }
    }
}
