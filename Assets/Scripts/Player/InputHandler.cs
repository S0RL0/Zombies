using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerControls playerControls;

    private PlayerController playerController;
    private WeaponSystem weaponSystem;

    void Start()
    {
        LockCursor(true);
    }

    void Awake()
    {
        playerControls = new PlayerControls();
        playerController = GetComponent<PlayerController>();
        weaponSystem = GetComponentInChildren<WeaponSystem>();
    }

    void OnEnable()
    {
        playerControls.Player.Enable();

        playerControls.Player.Move.performed += HandleMove;
        playerControls.Player.Move.canceled += HandleMove;
        playerControls.Player.Look.performed += HandleLook;
        playerControls.Player.Look.canceled += HandleLook;
        playerControls.Player.Jump.performed += ctx => playerController.isJumping = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.performed += ctx => playerController.isSprinting = ctx.ReadValueAsButton();
        playerControls.Player.Sprint.canceled += ctx => playerController.isSprinting = ctx.ReadValueAsButton();
        playerControls.Player.Crouch.performed += ctx => ToggleCrouch(ctx.ReadValueAsButton());
        playerControls.Player.Interact.performed += HandleInteract;
        if (weaponSystem != null)
        {
            // Fire
            playerControls.Player.Fire.performed += ctx =>
            {
                if (ctx.ReadValueAsButton())
                    weaponSystem.OnFirePerformed();
            };

            playerControls.Player.Fire.canceled += ctx =>
            {
                weaponSystem.OnFireCanceled();
            };

            // Reload
            playerControls.Player.Reload.performed += ctx =>
            {
                if (ctx.ReadValueAsButton())
                    weaponSystem.OnReloadPerformed();
            };
        }
        else
        {
            Debug.LogWarning("InputHandler: No WeaponSystem found in children.");
        }


    }

    void HandleMove(InputAction.CallbackContext context)
    {
        playerController.moveInput = context.ReadValue<Vector2>();
    }
    void HandleLook(InputAction.CallbackContext context)
    {
        playerController.lookInput = context.ReadValue<Vector2>();
    }
    void ToggleCrouch(bool isCrouchButtonPressed)
    {
        if (isCrouchButtonPressed)
        {
            playerController.Crouch();
        }
    }

    void HandleInteract(InputAction.CallbackContext context)
    {
        // Implement interaction logic here
        playerController.Interact();
    }

    private void LockCursor(bool lockState)
    {
        if (lockState)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
