
using System;
using Unity.Mathematics;
using UnityEngine;

namespace SoulHunter.Player
{
    public class PlayerMovement : MonoBehaviour, Input.IMoveInput // Mort
    {
        [Header("Movement Attributes")]
        public float moveSpeed = 3f;
        public float jumpSpeed = 8f;
        public float swingForce = 4f;
        public float yankForce = 7f;

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
        public void HandleMovement()
        {
            if (PlayerBase.isPaused)
            {
                return;
            }

            if (i_moveInput.x * Math.Sign(i_moveInput.x) > 0.01f)
            {
                if (PlayerBase.isSwinging)
                {
                    var playerToHookDirection = (PlayerBase.ropeHook - (Vector2)transform.position).normalized;

                    Vector2 perpendicularDirection;
                    if (i_moveInput.x < 0)
                    {
                        perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
                        var leftPerpPos = (Vector2)transform.position - perpendicularDirection * -2f;
                        Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
                    }
                    else
                    {
                        perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
                        var rightPerpPos = (Vector2)transform.position + perpendicularDirection * 2f;
                        Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
                    }

                    var force = perpendicularDirection * swingForce;
                    rigidBody.AddForce(force, ForceMode2D.Force);

                    if (rigidBody.velocity.x * Math.Sign(rigidBody.velocity.x) > 4)
                    {
                        AudioManager.PlaySound(AudioManager.Sound.ClothFlowing, transform.position);
                    }
                }
                else
                {
                    if (PlayerBase.isGrounded)
                    {
                        var groundForce = moveSpeed * 2f;
                        rigidBody.AddForce(new Vector2((i_moveInput.x * groundForce - rigidBody.velocity.x) * groundForce, 0));
                        rigidBody.velocity = new Vector2(velocity.x, velocity.y);
                        velocity.y = 0;

                        RaycastHit2D slopeHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .7f), Vector2.right * math.sign(velocity.x), 1.5f, groundCheckLayer);
                        if (slopeHit)
                        {
                            float slopeAngle = Vector2.Angle(slopeHit.normal, Vector2.up);

                            if (slopeAngle <= maxClimbAngle)
                            {
                                //print("Slope: " + slopeHit.transform.name + " - " + slopeAngle);
                                ClimbSlope(ref velocity, slopeAngle);
                            }
                        }

                        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundCheckLayer);
                        if (groundHit.collider != null)
                        {
                            if (groundHit.transform.gameObject.layer == 12)
                            {
                                AudioManager.PlaySound(AudioManager.Sound.PlayerWalkWood, transform.position);
                            }
                            else if (groundHit.transform.gameObject.layer == 13)
                            {
                                AudioManager.PlaySound(AudioManager.Sound.PlayerWalkGrass, transform.position);
                            }
                        }
                    }
                }
            }

            if (!PlayerBase.isSwinging)
            {
                if (!PlayerBase.isGrounded) return;

                if (PlayerBase.isJumping)
                {
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
                    AudioManager.PlaySound(AudioManager.Sound.PlayerJump, transform.position);
                }
            }
            else
            {
                if (PlayerBase.isJumping)
                {
                    if (PlayerBase.ropeHook != Vector2.zero)
                    {
                        rigidBody.AddForce((new Vector3(PlayerBase.ropeHook.x, PlayerBase.ropeHook.y, 0) - transform.position) * (yankForce * 10));
                        GetComponent<GrappleSystem>().ResetRope();
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