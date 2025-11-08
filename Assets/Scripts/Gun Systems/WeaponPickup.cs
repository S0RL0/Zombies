using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponProfile weaponProfile;
    public GameObject weaponPrefab;

    private Gun gunInstance;

    private void Awake()
    {
        // If prefab contains Gun.cs, disable it so it can't shoot while on floor
        gunInstance = GetComponent<Gun>();
        if (gunInstance != null)
            gunInstance.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeaponManager manager = other.GetComponent<PlayerWeaponManager>();
            if (manager != null)
            {
                // Disable this collider immediately to prevent multiple triggers
                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;

                // Pickup the weapon
                manager.PickupWeapon(this);

                // Destroy pickup object after equipping
                Destroy(gameObject);
            }
        }
    }

}
