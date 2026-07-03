using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulHunter.Gameplay;
using SoulHunter.Player;

namespace SoulHunter.Weapons 
{
    public class EnemyWeapon : MonoBehaviour
    {
        [SerializeField] int KnockBack;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerAnimation>())
            {
                PlayerBase PlayerHealth = collision.gameObject.GetComponentInParent<PlayerBase>();
                Rigidbody2D PlayerRB = collision.gameObject.GetComponentInParent<Rigidbody2D>();
                PlayerRB.AddForce(transform.up * 30);
                Vector2 WeaponOwnerPos = transform.parent.gameObject.transform.position;
                if (WeaponOwnerPos.x < transform.position.x)
                {
                    PlayerRB.AddForce(Vector2.right * KnockBack);
                }
                else
                {
                    PlayerRB.AddForce(Vector2.left * KnockBack);
                }
                PlayerHealth.TakeDamage();
            }
        }
    }
}



