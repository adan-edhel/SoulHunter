
using System;
using Unity.Mathematics;
using UnityEngine;

namespace SoulHunter
{
    public class PlayerMovement : MonoBehaviour, IMoveInput // Mort
    {
        [Header("Movement Attributes")]
        public float moveSpeed = 7f;
        public float jumpSpeed = 8f;
        public float swingForce = 4f;
        public float yankForce = 7f;

        [SerializeField] float groundAcceleration = 45.0f;
        [SerializeField] float airAcceleration = 18.0f;


        // Max Slope Climb Angle
        float maxClimbAngle = 50;

        // Jump Merchanics Values
        float fGroundedRememberTime = .2f;
        float fGroundedRemember;
        float fCutJumpHeight = .5f;

        // Ground Collision Checkers
        bool[] groundCollision = new bool[3];

        [Header("Object Variables")]
        [SerializeField] GameObject impactDustParticle;
        public LayerMask groundCheckLayer;
        Rigidbody2D rigidBody;

        // Movement Input Value
        [HideInInspector]
        public Vector2 i_moveInput;
        Vector2 velocity;
        Vector2 oldVelocity;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CheckForGround();

            velocity.x = i_moveInput.x * moveSpeed;

            if (Time.frameCount%5==0)
            {
                oldVelocity = rigidBody.velocity;
            }
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        /// <summary>
        /// Handles player movement and jumping using input interfaces
        /// </summary>
        private void HandleMovement()
        {
            if (PlayerBase.isPaused) return;

            float acceleration = PlayerBase.isGrounded ? groundAcceleration : airAcceleration;

            // HORIZONTAL MOVEMENT
            if (!PlayerBase.isSwinging)
            {
                float targetSpeed = i_moveInput.x * moveSpeed;
                float newXVelocity = Mathf.MoveTowards(rigidBody.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

                // Slope handling
                if (PlayerBase.isGrounded)
                {
                    RaycastHit2D slopeHit = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, groundCheckLayer);
                    if (slopeHit)
                    {
                        float slopeAngle = Vector2.Angle(slopeHit.normal, Vector2.up);
                        if (slopeAngle <= maxClimbAngle && slopeAngle > 0.1f)
                        {
                            newXVelocity = targetSpeed * Mathf.Cos(slopeAngle * Mathf.Deg2Rad);
                        }
                    }
                }

                rigidBody.velocity = new Vector2(newXVelocity, rigidBody.velocity.y);
            }
            else
            { // Swinging
                var playerToHookDirection = (PlayerBase.ropeHook - (Vector2)transform.position).normalized;
                Vector2 perpendicularDirection = i_moveInput.x < 0
                    ? new Vector2(-playerToHookDirection.y, playerToHookDirection.x)
                    : new Vector2(playerToHookDirection.y, -playerToHookDirection.x);

                rigidBody.AddForce(perpendicularDirection * swingForce, ForceMode2D.Force);

                if (Mathf.Abs(rigidBody.velocity.x) > 4f)
                    AudioManager.PlaySound(AudioManager.Sound.ClothFlowing, transform.position);
            }

            // JUMPING
            if (PlayerBase.isJumping)
            {
                if (!PlayerBase.isSwinging)
                {
                    if (PlayerBase.isGrounded)
                    {
                        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
                        AudioManager.PlaySound(AudioManager.Sound.PlayerJump, transform.position);
                        PlayerBase.isJumping = false;
                    }
                }
                else
                {
                    // Yank
                    if (PlayerBase.ropeHook != Vector2.zero)
                    {
                        rigidBody.AddForce((PlayerBase.ropeHook - (Vector2)transform.position).normalized * (yankForce * 12f));
                        GetComponent<GrappleSystem>().ResetRope();
                        PlayerBase.isJumping = false;
                    }
                }
            }
        }

        /// <summary>
        /// Compensates for the velocity at angles
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="slopeAngle"></param>
        void ClimbSlope(ref Vector2 velocity, float slopeAngle) // Made with the amazing tutorial of Sebastian Lague on youtube!
        {
            float moveDistance = Mathf.Abs(velocity.x);
            velocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
        }

        /// <summary>
        /// Cuts jumps in half if input is released
        /// </summary>
        public void CutJump()
        {
            if (rigidBody.velocity.y > 0)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y * fCutJumpHeight);
            }

            PlayerBase.isJumping = false;
        }

        /// <summary>
        /// Checks for ground colliders at the base of player
        /// </summary>
        void CheckForGround()
        {
            var halfHeight = PlayerBase.playerSprite.bounds.extents.y;
            groundCollision[0] = Physics2D.OverlapCircle(new Vector2(transform.position.x + 0.4f, transform.position.y - halfHeight), 0.1f, groundCheckLayer);
            groundCollision[1] = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - halfHeight), 0.1f, groundCheckLayer);
            groundCollision[2] = Physics2D.OverlapCircle(new Vector2(transform.position.x - 0.4f, transform.position.y - halfHeight), 0.1f, groundCheckLayer);

            fGroundedRemember -= Time.deltaTime;

            for (int i = 0; i < groundCollision.Length; i++)
            {
                if (groundCollision[i])
                {
                    fGroundedRemember = fGroundedRememberTime;
                    PlayerBase.isGrounded = true;

                    if (PlayerBase.isSwinging)
                    {
                        GetComponent<GrappleSystem>().ResetRope();
                    }

                    return;
                }

                if (fGroundedRemember < 0 && !groundCollision[i])
                {
                    PlayerBase.isGrounded = false;
                }
            }

            if (PlayerBase.isJumping)
            {
                fGroundedRemember = -1;
            }
        }

        /// <summary>
        /// Gets movement input and saves it in a local variable
        /// </summary>
        /// <param name="input"></param>
        public void HandleMoveInput(Vector2 input)
        {
            i_moveInput = input;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Ground Impact Particle
            if (oldVelocity.y < -5)
            {
                Instantiate(impactDustParticle, new Vector3(transform.position.x, transform.position.y - PlayerBase.playerSprite.bounds.extents.y, transform.position.z + 1) , impactDustParticle.transform.rotation);

                if (collision.transform.gameObject.layer == 12)
                {
                    AudioManager.PlaySound(AudioManager.Sound.PlayerLandWood, transform.position);
                }
                else if (collision.transform.gameObject.layer == 13)
                {
                    AudioManager.PlaySound(AudioManager.Sound.PlayerLandGrass, transform.position);
                }
            }
            
            // Ground Impact Shake
            if (oldVelocity.y < -8)
            {
                if (oldVelocity.y < -12)
                {
                    CameraManager.Instance.ShakeCamera(2, 6, 0);
                }
                else
                {
                    CameraManager.Instance.ShakeCamera(1, 0, 0);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (PlayerBase.playerSprite)
            {
                Gizmos.color = Color.blue;
                var halfHeight = PlayerBase.playerSprite.bounds.extents.y;
                Gizmos.DrawWireSphere(new Vector3(transform.position.x + 0.4f, transform.position.y - halfHeight, -2), 0.1f);
                Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - halfHeight, -2), 0.1f);
                Gizmos.DrawWireSphere(new Vector3(transform.position.x - 0.4f, transform.position.y - halfHeight, -2), 0.1f);
            }

            Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y - .7f), Vector2.right * Mathf.Sign(velocity.x));
        }
    }
}