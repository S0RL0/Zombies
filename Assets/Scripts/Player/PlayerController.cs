using UnityEngine;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private CharacterController characterController;
    private WeaponSystem weaponSystem;

    [Header("Stats")]
    [SerializeField] private float lookSpeed = 100f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 1.8f;

    // Input varibles
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lookInput = Vector2.zero;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isCrouching;

    // Logic variables
    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform cameraTransform;
    private float verticalLookRotation;

    // Economy System
    public int money = 0;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Camera _camera = GetComponentInChildren<Camera>();
        cameraTransform = _camera.gameObject.transform;
        weaponSystem = GetComponent<WeaponSystem>();
    }


    void FixedUpdate()
    {

        Move();

        Jump();

        Look();


    }

    public void ToggleCrouch(bool isCrouchButtonPressed)
    {
        if (isCrouchButtonPressed)
        {
            isCrouching = !isCrouching;
            characterController.height = isCrouching ? crouchHeight : standingHeight;
        }
    }

    private void Move()
    {
        // Calculate movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        characterController.Move(move * speed * Time.deltaTime);
    }

    private void Jump()
    {
        // Un Crouch if crounched
        if (isCrouching && isJumping)
        {
            isJumping = false;
            Crouch();
        }

        // Jump
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = false;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Ground Check
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }


    }

    private void Look()
    {

        // Inputs
        float horizontalLook = lookInput.x * lookSpeed * Time.deltaTime;
        float verticalLook = lookInput.y * lookSpeed * Time.deltaTime;

        // Vertical rotation
        verticalLookRotation -= verticalLook;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        // Rotatation
        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
        transform.Rotate(Vector3.up * horizontalLook);
    }

    public void Crouch()
    {
        isCrouching = !isCrouching;
        float currentHeight = isCrouching ? crouchHeight : standingHeight;
        characterController.height = currentHeight;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, currentHeight, cameraTransform.localPosition.z);
    }

    public void Interact()
    {
        Debug.LogWarning("Interacting...");
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 3f))
        {
            Debug.LogWarning("Hit: " + hit.collider.name);
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.LogWarning("Found Interactable: " + hit.collider.name);
                InteractionResult result = interactable.Interact();
                if (result.success)
                {
                    Debug.LogWarning("Interaction Successful with: " + hit.collider.name);
                    InteractionType item = result.interactionType;
                    switch(item)
                    {
                        case InteractionType.Weapon:
                            weaponSystem.PickupNewWeapon(result.GetItem<WeaponProfile>(), result.sourceObject, result.sourceObject.GetComponent<Weapon>());
                            break;
                        case InteractionType.Door:
                            break;
                        case InteractionType.Buy:
                            Debug.Log("Attempting to buy " + result.GetItem<WeaponProfile>().name + " for " + result.GetItem<WeaponProfile>().cost);
                            money -= result.GetItem<WeaponProfile>().cost;
                            // Sound here, successful purchase
                            weaponSystem.PickupNewWeapon(result.GetItem<WeaponProfile>(), result.sourceObject, result.sourceObject.GetComponent<Weapon>());
                            break;
                        case InteractionType.Ammo:
                            money -= result.GetItem<WeaponProfile>().ammoCost;
                            // Sound here, successful purchase
                            weaponSystem.RefillAmmo(result.GetItem<WeaponProfile>());
                            break;

                        default:
                            Debug.Log("Interacted with " + hit.collider.name);
                            break;
                    }
                }
            }
        }
    }

    public bool HasWeapon(WeaponProfile weaponToCheck)
    {
        return weaponSystem.profiles.Contains(weaponToCheck);
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
}
