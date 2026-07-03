using UnityEngine;

using SoulHunter.Enemy;

namespace SoulHunter.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] float attackDuration;
        public GameObject Weapon;
        float attackTimer = 2f;

        void Start()
        {
            Weapon.SetActive(false);
            attackDuration = attackDuration / 10;
        }

        void Update()
        {
            if (PlayerBase.isAttacking == true)
            {
                attackTimer += Time.deltaTime;
            }

            ResetWeapon();
        }

        public void Attack()
        {
            if (PlayerBase.isSwinging || PlayerBase.isAttacking)
            {
                return;
            }

            if (PlayerBase.playerSprite.flipX == true)
            {   // Left
                Weapon.transform.localPosition = new Vector3(-1, 0, 0);
            }
            else
            {   // Right
                Weapon.transform.localPosition = new Vector3(1, 0, 0);
            }
            GetComponentInChildren<PlayerAnimation>().FlipSlashSprites();
            Weapon.SetActive(true);
            PlayerBase.isAttacking = true;
        }

        void ResetWeapon()
        {
            if (attackTimer >= attackDuration)
            {
                Weapon.SetActive(false);
                attackTimer = 0;
                PlayerBase.isAttacking = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (!other.GetComponentInParent<EnemyBase>().isDead)
                {
                    other.GetComponentInParent<EnemyCombat>().Attack();
                }
            }
        }
    }
}