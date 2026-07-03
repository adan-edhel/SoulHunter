using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SoulHunter.Gameplay;
using SoulHunter.Enemy;

namespace SoulHunter.Weapons
{
    public class WeaponScript : MonoBehaviour
    {
        [SerializeField] float Knockback;
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (collision.GetComponent<SpriteRenderer>())
                {
                    EnemyBase EnemyHealth = collision.gameObject.GetComponentInParent<EnemyBase>();
                    Rigidbody2D EnemyRB = collision.gameObject.GetComponentInParent<Rigidbody2D>();
                    EnemyRB.AddForce(transform.up * 100);
                    Vector2 WeaponOwnerPos = transform.parent.gameObject.transform.position;
                    if (WeaponOwnerPos.x < transform.position.x)
                    {
                        EnemyRB.AddForce(Vector2.right * Knockback);
                    }
                    else
                    {
                        EnemyRB.AddForce(Vector2.left * Knockback);
                    }
                    EnemyHealth.TakeDamage();
                }
                AudioManager.PlaySound(AudioManager.Sound.PlayerAttackHit, transform.position);
            }
            else
            {
                AudioManager.PlaySound(AudioManager.Sound.PlayerAttackMiss, transform.position);
            }
        }
    }
}