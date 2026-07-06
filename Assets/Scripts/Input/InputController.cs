using UnityEngine.InputSystem;
using UnityEngine;

using SoulHunter.Player;

namespace SoulHunter.Input
{
    public class InputController : MonoBehaviour // Mort
    {
        // Singleton Instance
        public static InputController Instance;
        
        //Interfaces
        IMoveInput[] i_MoveInput;
        IAimInput i_AimInput;
        public ITogglePause i_TogglePause;

        //Scripts
        PlayerCombat playerCombat;
        GrappleSystem grappleSystem;
        PlayerMovement playerMovement;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            //Get Interfaces
            i_MoveInput = GetComponents<IMoveInput>();
            i_AimInput = GetComponent<IAimInput>();

            //Get Scripts
            playerCombat = GetComponent<PlayerCombat>();
            grappleSystem = GetComponent<GrappleSystem>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        /// <summary>
        /// Gets aim input and sends it through an interface
        /// </summary>
        /// <param name="context"></param>
        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.action.ReadValue<Vector2>() != Vector2.zero)
            {
                i_AimInput?.HandleAimInput(context.action.ReadValue<Vector2>());
            }
        }

        /// <summary>
        /// Gets movement input and sends it through an interface
        /// </summary>
        /// <param name="context"></param>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                for (int i = 0; i < i_MoveInput.Length; i++)
                {
                    i_MoveInput[i]?.HandleMoveInput(context.action.ReadValue<Vector2>());
                }
            }
        }

        /// <summary>
        /// Gets jump input and sets its bool
        /// </summary>
        /// <param name="context"></param>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (PlayerBase.isPaused)
            {
                return;
            }

            if (context.performed)
            {
                if (playerMovement)
                {
                    PlayerBase.isJumping = true;
                }
            }

            if (context.canceled)
            {
                playerMovement?.CutJump();
            }
        }

        /// <summary>
        /// Gets interact input and sets its bool
        /// </summary>
        /// <param name="context"></param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GameManager.interacting = true;
            }

            if (context.canceled)
            {
                GameManager.interacting = false;
            }
        }

        /// <summary>
        /// Gets attack input and calls its function
        /// </summary>
        /// <param name="context"></param>
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (PlayerBase.isPaused)
            {
                return;
            }

            if (context.performed)
            {
                playerCombat?.Attack();
            }
        }

        /// <summary>
        /// Gets pause input and sets its bool
        /// </summary>
        /// <param name="context"></param>
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                i_TogglePause?.TogglePause();
            }
        }

        /// <summary>
        /// Gets swing input and calls its function
        /// </summary>
        /// <param name="context"></param>
        public void OnGrapple(InputAction.CallbackContext context)
        {
            if (PlayerBase.isPaused)
            {
                return;
            }

            if (context.performed)
            {
                if (!PlayerBase.isSwinging)
                {
                    grappleSystem?.ShootGrapple(GetComponent<PlayerAim>().aimDirection);
                }
                else
                {
                    grappleSystem?.ResetRope();
                }
            }
        }
    }
}