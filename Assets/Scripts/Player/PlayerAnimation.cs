using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulHunter.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        PlayerMovement movement;
        Rigidbody2D rigidBody;

        Animator anim;

        [SerializeField] GameObject LeftSlash;
        [SerializeField] GameObject RightSlash;

        void Start()
        {
            PlayerBase.playerSprite = GetComponent<SpriteRenderer>();

            rigidBody = GetComponentInParent<Rigidbody2D>();
            movement = GetComponentInParent<PlayerMovement>();
            anim = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            UpdateValues();
            FlipTexture();
        }

        /// <summary>
        /// Updates animator values
        /// </summary>
        void UpdateValues()
        {
            // Syncs animator booleans with static booleans
            anim.SetBool("isGrounded", PlayerBase.isGrounded);
            anim.SetBool("isAttacking", PlayerBase.isAttacking);
            anim.SetBool("isSwinging", PlayerBase.isSwinging);
            anim.SetBool("isJumping", PlayerBase.isJumping);
            anim.SetBool("isThrowing", PlayerBase.isThrowing);

            // If player is on the ground and game isn't pause, animate walking
            if (PlayerBase.isGrounded && !PlayerBase.isPaused)
            {
                anim.SetFloat("Speed", Mathf.Abs(movement.i_moveInput.x));
            }
            else // Otherwise, don't animate walking
            {
                anim.SetFloat("Speed", 0);
            }
        }

        /// <summary>
        /// Flip player texture according to movement on the X axis
        /// </summary>
        void FlipTexture()
        {
            if (PlayerBase.isGrounded && !PlayerBase.isPaused)
            {
                if (movement.i_moveInput.x < 0)
                {   // Direction Left
                    PlayerBase.playerSprite.flipX = true;
                    LeftSlash.SetActive(true);
                    RightSlash.SetActive(false);
                }
                else if (movement.i_moveInput.x > 0)
                {   // Direction Right
                    PlayerBase.playerSprite.flipX = false;
                    LeftSlash.SetActive(false);
                    RightSlash.SetActive(true);
                }
            }
            else
            {
                if (rigidBody.velocity.x < 0)
                {   // turn left
                    PlayerBase.playerSprite.flipX = true;
                }
                else if (rigidBody.velocity.x > 0)
                {   // turn right
                    PlayerBase.playerSprite.flipX = false;
                }
            }
        }

        /// <summary>
        /// Variate slash textures
        /// </summary>
        public void FlipSlashSprites()
        {
            RightSlash.GetComponent<SpriteRenderer>().flipY = !RightSlash.GetComponent<SpriteRenderer>().flipY;
            LeftSlash.GetComponent<SpriteRenderer>().flipY = !LeftSlash.GetComponent<SpriteRenderer>().flipY;
        }

        /// <summary>
        /// Resets throwing after animation
        /// </summary>
        public void ResetThrowing()
        {
            PlayerBase.isThrowing = false;
        }
    }
}