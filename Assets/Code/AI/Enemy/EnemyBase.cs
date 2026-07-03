using UnityEngine;
using SoulHunter.Gameplay;

namespace SoulHunter.Enemy
{
    public class EnemyBase : Damageable
    {
        EnemyAnimation animationOfEnemy;
        public SoulData soul;

        protected void Start()
        {
            GameManager.Instance.EnemyListRegistry(this);
            animationOfEnemy = GetComponentInChildren<EnemyAnimation>();
        }

        public override void TakeDamage()
        {
            base.TakeDamage();

            animationOfEnemy.HurtAnimation();

            if (!isDead)
            {
                AudioManager.PlaySound(AudioManager.Sound.EnemyHurt, transform.position);
            }
            else
            {
                if (!immuneToDamage)
                {
                    immuneToDamage = true;

                    GetComponent<Collider2D>().enabled = false;

                    AudioManager.PlaySound(AudioManager.Sound.EnemyDeath, transform.position);
                    soul.SpawnParticle(transform.position);
                }
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.EnemyListRegistry(this);
        }
    }
}

