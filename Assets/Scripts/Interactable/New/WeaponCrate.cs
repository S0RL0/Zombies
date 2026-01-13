using UnityEngine;

public class WeaponCrate : Interactable
{
    public WeaponProfile profile;
    //public GameObject ammoModel;
    public GameObject weaponDisplayPoint;
    public GameObject model;
    public string interactPromptText;
    //public string interactAmmoText;
    protected PlayerController player;
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
            Invoke("CreateWeapon", 5f); // respawn gun after 5 seconds
            return new InteractionResult(true, profile, model, InteractionType.Buy);
        }
        else
        {
            return new InteractionResult(false, null, null, InteractionType.Buy);
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        CreateWeapon();
        player = FindFirstObjectByType<PlayerController>();
        if (profile != null)
            interactPromptText = $"for {profile.name} [Cost: ${profile.cost}]";
    }
    protected virtual void CreateWeapon()
    {
        model = Instantiate(profile.prefab, weaponDisplayPoint.transform.position, weaponDisplayPoint.transform.rotation, weaponDisplayPoint.transform);
        MeshCollider col = model.GetComponent<MeshCollider>();
        col.enabled = false;
        model.GetComponent<Rigidbody>().ToggleRB(false);
    }
    protected virtual void CreateWeapon(WeaponProfile _profile)
    {
        model = Instantiate(_profile.prefab, weaponDisplayPoint.transform.position, weaponDisplayPoint.transform.rotation, weaponDisplayPoint.transform);
        MeshCollider col = model.GetComponent<MeshCollider>();
        col.enabled = false;
        model.GetComponent<Rigidbody>().ToggleRB(false);
    }

    private void CreateAmmo()
    {

    }

    public override string GetInteractionText()
    {
        if (player.HasWeapon(profile))
        {
            return $"For Ammo [Cost: ${profile.ammoCost}]";
        }
        return interactPromptText;
    }

    public override Sprite GetInteractionIcon()
    {
        return profile.icon;
    }
}
