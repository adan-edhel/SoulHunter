using UnityEngine;

using SoulHunter.UI;
using SoulHunter.Gameplay;

namespace SoulHunter.Player
{
    public class PlayerBase : Damageable // Mort
    {
        GameUI healthUI; // <- Thomas

        // Player states
        public static bool isPaused;
        public static bool isTeleporting;

        public static bool isGrounded;
        public static bool isAttacking;
        public static bool isSwinging;
        public static bool isJumping;
        public static bool isThrowing;

        // Player sprite
        public static SpriteRenderer playerSprite;

        // Hook Position
        [HideInInspector]
        public static Vector2 ropeHook;

        // Teleportation coordinates
        public static Transform teleportDestination;

        // Spirit Anchor
        [SerializeField] GameObject spiritAnchor;

        protected void Start()
        {
            timer = 1f;
            healthUI = FindObjectOfType<GameUI>(); // <- Thomas
            SpiritOfTheWoods.instance.spiritAnchor = spiritAnchor;
        }

        protected override void Update()
        {
            base.Update();
            Dissolve();

            if (isTeleporting || isDead)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    timer = 0f;
                    if (!isDead)
                    {
                        Teleport();
                    }
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= 1f)
                {
                    timer = 1f;
                }
                else
                {
                    AudioManager.PlaySound(AudioManager.Sound.TeleportAppear, transform.position);
                }
            }
        }

        /// <summary>
        /// Syncs shader value with despawn value
        /// </summary>
        void Dissolve()
        {
            characterSprite.material.SetFloat("_Fade", timer);
        }

        /// <summary>
        /// Teleports player to the next destination
        /// </summary>
        void Teleport()
        {
            transform.position = new Vector3(teleportDestination.position.x, teleportDestination.position.y, 0);
            isTeleporting = false;
            isPaused = false;
        }

        /// <summary>
        /// Overrides default TakeDamage value to update UI and play unique sounds
        /// </summary>
        public override void TakeDamage()
        {
            base.TakeDamage();

            if (!isDead)
            {
                AudioManager.PlaySound(AudioManager.Sound.PlayerHurt, transform.position);
            }
            else
            {
                AudioManager.PlaySound(AudioManager.Sound.PlayerDeath, transform.position);
            }

            CameraManager.Instance.ShakeCamera(1, 3, 0);

            healthUI.UpdateHealthPanel(Health); // <- Thomas
        }

        /// <summary>
        /// Heals the player
        /// </summary>
        public void Heal()
        {
            if (Health >= maxHealth)
            {
                return;
            }

            Health++;
            healthUI.UpdateHealthPanel(Health);
        }

        /// <summary>
        /// Handles player death by restarting scene
        /// </summary>
        protected override void HandleDeath()
        {
            if (isDead && !isTeleporting)
            {
                isPaused = false;
                gameObject.SetActive(false);
                SceneController.Instance.ResetScene();
            }
        }
    }
}