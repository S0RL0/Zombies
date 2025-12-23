using UnityEngine;

public class WeaponCrate : Interactable
{
    public WeaponProfile profile;
    //public GameObject ammoModel;
    public GameObject weaponDisplayPoint;
    private GameObject model;

    private PlayerController player;
    public override InteractionResult Interact()
    {
        // Check if player is null
        if (player == null) return new InteractionResult(false, null, null, InteractionType.Buy);

        // Check if player already has the weapon
        if (player.HasWeapon(profile))
        {
            if (player.GetMoney() >= profile.ammoCost)
            {
                return new InteractionResult(true, profile, null, InteractionType.Ammo);
            }
        }
        // Check if player has enough money
        if (player.GetMoney() >= profile.cost)
        {
            Invoke("CreateGun", 5f); // respawn gun after 5 seconds
            return new InteractionResult(true, profile, model, InteractionType.Buy);
        }
        else
        {
            return new InteractionResult(false, null, null, InteractionType.Buy);
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateGun();
        player = FindFirstObjectByType<PlayerController>();
        Debug.Log("Player found: " + (player != null));
    }
    private void CreateGun()
    {
        model = Instantiate(profile.prefab, weaponDisplayPoint.transform.position, weaponDisplayPoint.transform.rotation, weaponDisplayPoint.transform);
        MeshCollider col = model.GetComponent<MeshCollider>();
        col.enabled = false;
        ToggleRB(model.GetComponent<Rigidbody>(), false);
    }

    private void CreateAmmo()
    {

    }

    private bool? ToggleRB(Rigidbody rb, bool enabled)
    {
        if (rb == null) return null;

        if (!enabled) // turn physics OFF
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = false; // optional
            return enabled;
        }
        else // turn physics ON
        {
            rb.isKinematic = false;
            rb.useGravity = true; // set how you want when re-enabled
            rb.detectCollisions = true; // optional
            return true;
        }
    }
}
