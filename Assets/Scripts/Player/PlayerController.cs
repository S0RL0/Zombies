using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("References")]
    private CharacterController characterController;
    private WeaponSystem weaponSystem;
    private Transform cameraTransform;

    [Header("Stats")]
    [SerializeField] private float lookSpeed = 20f;
    [SerializeField] private float smoothTime = 0.05f;
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
    [HideInInspector] public bool isFiring;
    [HideInInspector] public bool wasFiring;

    // Camera variables
    private Vector2 baseRotation;        // Player-controlled rotation (yaw, pitch)
    private Vector2 finalRotation;       // baseRotation + recoil offset
    private Vector2 recoilTarget;        // Target recoil offset when firing
    private Vector2 recoilOffset;        // Current recoil offset
    private Vector2 recoilVelocity;      // For smoothing recoil return
    private Vector2 recoilOrigin;        // Original rotation before recoil
    [SerializeField] private float recoilSnappiness = 10f; // How quickly the camera returns to original position after recoil
    [SerializeField] private float recoilOffsetVelocity;

    // Recoil recovery evaluation thresholds
    private float recoveryThreshold = 5f;

    enum RecoveryMode { ToZero, ToOrigin, None }
    private RecoveryMode recoilRecoveryMode;

    // Movement variables
    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;

    // Economy System
    public int money = 0;

    #endregion

    #region Awake
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Camera _camera = GetComponentInChildren<Camera>();
        cameraTransform = transform.Find("CameraRotation");
        weaponSystem = GetComponent<WeaponSystem>();
    }
    #endregion

    #region Update
    void Update()
    {
        Look();

        Camera();
    }

    void FixedUpdate()
    {

        Move();

        Jump();

    }
    #endregion

    #region Movement

    #region Move
    private void Move()
    {
        // Calculate movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        characterController.Move(move * speed * Time.deltaTime);
    }
    #endregion

    #region Jump
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
    #endregion

    #region Crouch
    public void Crouch()
    {
        isCrouching = !isCrouching;
        float currentHeight = isCrouching ? crouchHeight : standingHeight;
        characterController.height = currentHeight;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, currentHeight, cameraTransform.localPosition.z);
    }
    public void ToggleCrouch(bool isCrouchButtonPressed)
    {
        if (isCrouchButtonPressed)
        {
            isCrouching = !isCrouching;
            characterController.height = isCrouching ? crouchHeight : standingHeight;
        }
    }

    #endregion

    #endregion

    #region Camera
    private void Look()
    {
        // Inputs
        float horizontalLook = lookInput.x * lookSpeed * 0.01f;
        float verticalLook = lookInput.y * lookSpeed * 0.01f;


        // Base Rotation
        baseRotation.x += horizontalLook;
        baseRotation.y -= verticalLook;
        baseRotation.y = Mathf.Clamp(baseRotation.y, -90f, 90f);
    }

    private void Camera()
    {
        recoilOffset = Vector2.Lerp(recoilOffset, recoilTarget, Time.deltaTime * recoilSnappiness);

        if (!isFiring && recoilOffset.magnitude > 0.01f) ResetRecoil();

        if (!isFiring && (recoilOffset - recoilTarget).magnitude < 0.01f && recoilOffset.magnitude != 0) BakeRecoilIntoBase();

        // Update camera rotation with recoil
        finalRotation = baseRotation + recoilOffset;
        finalRotation.y = Mathf.Clamp(finalRotation.y, -90f, 90f);
        transform.rotation = Quaternion.Euler(0f, finalRotation.x, 0f);
        cameraTransform.localRotation = Quaternion.Euler(finalRotation.y, 0f, 0f);
    }

    public void AddRecoil(Vector2 recoilAmount)
    {
        recoilTarget += recoilAmount;
    }

    public void ResetRecoil()
    {
        Vector2 target = Vector2.zero;

        switch (recoilRecoveryMode)
        {
            case RecoveryMode.ToZero:
                target = Vector2.zero;
                break;

            case RecoveryMode.ToOrigin:
                target = recoilOrigin - baseRotation;
                break;

            case RecoveryMode.None:
                BakeRecoilIntoBase();
                return; // Do not apply any recoil recovery
        }
        float resetTime = recoilRecoveryMode == RecoveryMode.ToZero ? 0.3f : 0.1f; // Faster reset for smaller offsets

        recoilTarget = Vector2.SmoothDamp(
            recoilTarget,
            target,
            ref recoilVelocity,
            resetTime
        );
    }

    void BakeRecoilIntoBase()
    {
        baseRotation += recoilOffset;

        recoilOffset = Vector2.zero;
        recoilTarget = Vector2.zero;
        recoilVelocity = Vector2.zero;

    }

    public void StartShooting()
    {
        BakeRecoilIntoBase();
        recoilOrigin = baseRotation;
        isFiring = true;
    }

    public void StopShooting()
    {
        isFiring = false;

        EvaluateRecoilRecovery();
    }

    void EvaluateRecoilRecovery()
    {
        // Difference between where the player started firing and where they are now (excluding recoil)
        Vector2 recoilessDeviation = baseRotation - recoilOrigin;
        float recoilessDeviationMagnitude = recoilessDeviation.magnitude;

        Vector2 recoilDeviation = finalRotation - recoilOrigin;
        float recoilDeviationMagnitude = recoilDeviation.magnitude;

        // Player has not moved the crosshair much since firing
        if (recoilessDeviationMagnitude < recoveryThreshold)
        {
            // Case 1: burst / no control
            //Debug.Log("No control detected. Recoil will return to zero. Recoiless Deviation Magnitude: " + recoilessDeviationMagnitude);
            recoilRecoveryMode = RecoveryMode.ToZero;
        }
        // Crosshair is close to original target, the player has good control
        else if (recoilDeviationMagnitude < recoveryThreshold * 2)
        {
            // Case 2: good control
            //Debug.Log("Good control detected. Recoil will return to origin. Recoil Deviation Magnitude: " + recoilDeviationMagnitude);
            recoilRecoveryMode = RecoveryMode.ToOrigin;
        }
        else
        {
            // Case 3: bad control
            //Debug.Log("Movement under aiming detected. Recoil will return vertically only. Recoiless Deviation Magnitude: " + recoilessDeviationMagnitude + "| Recoil Deviation Magnitude: " + recoilDeviationMagnitude);
            recoilRecoveryMode = RecoveryMode.None;
        }
    }
    #endregion

    #region Interaction
    public void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 3f))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                InteractionResult result = interactable.Interact();
                if (result.success)
                {
                    Debug.Log("Interaction Successful with: " + hit.collider.name);
                    InteractionType item = result.interactionType;
                    switch (item)
                    {
                        case InteractionType.Weapon:
                            weaponSystem.PickupNewWeapon(result.GetItem<WeaponProfile>(), result.sourceObject, result.sourceObject.GetComponent<Weapon>());
                            break;
                        case InteractionType.Door:
                            object _item = result.item;
                            if (_item is int cost)
                                money -= cost;
                            break;
                        case InteractionType.Buy:
                            money -= result.GetItem<WeaponProfile>().cost;
                            // Sound here, successful purchase
                            weaponSystem.PickupNewWeapon(result.GetItem<WeaponProfile>(), result.sourceObject, result.sourceObject.GetComponent<Weapon>());
                            break;
                        case InteractionType.Ammo:
                            money -= result.GetItem<WeaponProfile>().ammoCost;
                            // Sound here, successful purchase
                            weaponSystem.RefillAmmo(result.GetItem<WeaponProfile>());
                            break;
                        case InteractionType.Mystery:
                            object box = result.item;
                            if (box is int boxCost)
                                money -= boxCost;
                            break;
                        case InteractionType.Upgrade:
                            object upgrade = result.item;
                            if (upgrade is int upgradeCost)
                                money -= upgradeCost;
                            break;
                        default:
                            Debug.Log("Interacted with " + hit.collider.name);
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #region Inventory

    #region Weapons
    public bool HasWeapon(WeaponProfile weaponToCheck)
    {
        return weaponSystem.profiles.Contains(weaponToCheck);
    }
    public bool HasWeapon()
    {
        return weaponSystem.profiles.Count > 0;
    }

    public WeaponProfile GetCurrentWeaponProfile()
    {
        return weaponSystem.GetCurrentWeaponProfile();
    }

    public GameObject GetDropedWeapon()
    {
        return weaponSystem.DropCurrentWeapon(false);
    }
    #endregion

    #region Money
    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
    #endregion

    #endregion
}
