using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerControls playerControls;

    private PlayerController playerController;

    void Awake()
    {
        playerControls = new PlayerControls();
        playerController = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        playerControls.Player.Enable();

        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;
        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Look.canceled += OnLook;
        //playerControls.Player.Jump.performed += ctx => playerController.isJumping = ctx.ReadValueAsButton();
        //playerControls.Player.Sprint.performed += ctx => playerController.isSprinting = ctx.ReadValueAsButton();
        //playerControls.Player.Sprint.canceled += ctx => playerController.isSprinting = ctx.ReadValueAsButton();
        //playerControls.Player.Crouch.performed += ctx => ToggleCrouch(ctx.ReadValueAsButton());


    }

    void OnMove(InputAction.CallbackContext context)
    {
        //playerController.moveInput = context.ReadValue<Vector2>();
    }
    void OnLook(InputAction.CallbackContext context)
    {
        //playerController.lookInput = context.ReadValue<Vector2>();
    }
}
