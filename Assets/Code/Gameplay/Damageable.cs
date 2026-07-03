using UnityEngine;

namespace SoulHunter.Gameplay
{
    public class Damageable : MonoBehaviour // Mort & Thomas
    {
        // Max HP constant value
        public int maxHealth = 3;

        // HP value
        [HideInInspector]
        public int Health;

        // Death status
        public bool isDead;
        public bool immuneToDamage;

        // Height at which entities expire
        [SerializeField] protected float expirationHeight = -10;
        // Duration in which dead entities persist
        [SerializeField] protected float timer = 10;

        // Entity sprite reference
        [SerializeField] protected SpriteRenderer characterSprite;

        private void Awake()
        {
            // Set HP value to Max HP value at start
            Health = maxHealth;
        }

        protected virtual void Update()
        {
            HandleFallDeath();

            HandleDespawn();

            if (isDead && timer <= 0f)
            {
                HandleDeath();
            }
        }

        /// <summary>
        /// Delivers damage to the entity
        /// </summary>
        public virtual void TakeDamage()
        {
            Health--;

            if (Health < 1)
            {
                isDead = true;
            }
        }

        /// <summary>
        /// Handles destruction of entities below expiration height
        /// </summary>
        void HandleFallDeath()
        {
            if (transform.position.y <= expirationHeight)
            {
                isDead = true;
            }
        }

        /// <summary>
        /// Counts down despawn timer once entity is dead
        /// </summary>
        void HandleDespawn()
        {
            if (isDead)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    timer = 0f;
                }
            }
        }

        /// <summary>
        /// Destroys entity
        /// </summary>
        protected virtual void HandleDeath()
        {
            Destroy(gameObject);
        }
    }
}