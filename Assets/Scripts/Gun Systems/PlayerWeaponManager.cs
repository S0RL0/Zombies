using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Hold Points")]
    public Transform primaryHolder;
    public Transform secondaryHolder;
    public Transform dropPoint; // optional, for dropped weapon spawn

    [Header("References")]
    public Transform cameraTransform;

    private Gun primaryGun;
    private Gun secondaryGun;

    private WeaponProfile primaryProfile;
    private WeaponProfile secondaryProfile;

    private bool isPrimaryEquipped = true;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.SwapWeapon.performed += ctx => {
            Debug.Log("Swap pressed");
            SwapWeapons();
        };

        controls.Gameplay.DropWeapon.performed += ctx => {
            Debug.Log("Drop pressed");
            DropCurrentWeapon();
        };

    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        // Ensure proper visibility if guns exist at start
        UpdateWeaponVisibility();
    }

    #region Equip Weapon

    public void EquipWeapon(WeaponProfile profile, GameObject prefab)
    {
        // Decide which slot to equip
        bool equipPrimary = (primaryProfile == null);

        if (equipPrimary)
        {
            EquipToSlot(ref primaryGun, ref primaryProfile, primaryHolder, profile, prefab);
            isPrimaryEquipped = true;
        }
        else if (secondaryProfile == null)
        {
            EquipToSlot(ref secondaryGun, ref secondaryProfile, secondaryHolder, profile, prefab);
            isPrimaryEquipped = false;
        }
        else
        {
            // Swap out currently equipped weapon
            if (isPrimaryEquipped)
            {
                DropWeapon(primaryGun, primaryProfile, ref primaryGun, ref primaryProfile);
                EquipToSlot(ref primaryGun, ref primaryProfile, primaryHolder, profile, prefab);
            }
            else
            {
                DropWeapon(secondaryGun, secondaryProfile, ref secondaryGun, ref secondaryProfile);
                EquipToSlot(ref secondaryGun, ref secondaryProfile, secondaryHolder, profile, prefab);
            }
        }

        // Ensure correct visibility after equipping
        UpdateWeaponVisibility();
    }

    private void EquipToSlot(ref Gun gunSlot, ref WeaponProfile profileSlot, Transform holder, WeaponProfile profile, GameObject prefab)
    {
         if (prefab == null)
    {
        Debug.LogError("EquipToSlot failed: prefab is null!");
        return;
    }

    if (holder == null)
    {
        Debug.LogError("EquipToSlot failed: holder is null!");
        return;
    }

    GameObject gunObj = Instantiate(prefab, holder.position, holder.rotation, holder);
    gunSlot = gunObj.GetComponent<Gun>();

    if (gunSlot == null)
    {
        Debug.LogError("EquipToSlot failed: Gun component missing on prefab!");
        return;
    }

    gunSlot.profile = profile;
    gunSlot.OnEquipped();
    gunSlot.gameObject.SetActive(false);

    profileSlot = profile;
    }


    #endregion

    #region Swap Weapons

    public void SwapWeapons()
    {
        // Only swap if both guns exist
        if (primaryGun == null || secondaryGun == null) return;

        isPrimaryEquipped = !isPrimaryEquipped;
        UpdateWeaponVisibility();
    }

    private void UpdateWeaponVisibility()
    {
        if (primaryGun) primaryGun.gameObject.SetActive(isPrimaryEquipped);
        if (secondaryGun) secondaryGun.gameObject.SetActive(!isPrimaryEquipped);
    }

    #endregion

    #region Drop Weapon

    public void DropCurrentWeapon()
    {
        if (isPrimaryEquipped)
            DropWeapon(primaryGun, primaryProfile, ref primaryGun, ref primaryProfile);
        else
            DropWeapon(secondaryGun, secondaryProfile, ref secondaryGun, ref secondaryProfile);

        UpdateWeaponVisibility();
    }

    private void DropWeapon(Gun gun, WeaponProfile profile, ref Gun gunSlot, ref WeaponProfile profileSlot)
    {
        if (!gun) return;

        gun.transform.parent = null;

        if (!gun.TryGetComponent<Rigidbody>(out Rigidbody rb))
            rb = gun.gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;

        if (!gun.TryGetComponent<Collider>(out Collider col))
            gun.gameObject.AddComponent<BoxCollider>();

        // Optional: throw forward
        rb.AddForce(cameraTransform.forward * 2f, ForceMode.Impulse);

        // Disable Gun component so it cannot shoot while on floor
        gun.enabled = false;

        gunSlot = null;
        profileSlot = null;
    }

    #endregion

    #region Pickup

    public void PickupWeapon(WeaponPickup pickup)
    {
        EquipWeapon(pickup.weaponProfile, pickup.weaponPrefab);
        Destroy(pickup.gameObject);
    }

    #endregion

    #region Accessor

    public Gun GetCurrentGun()
    {
        return isPrimaryEquipped ? primaryGun : secondaryGun;
    }

    #endregion
}
