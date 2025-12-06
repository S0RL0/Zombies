using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

public class Weapon : Interactable
{
    public WeaponProfile weaponProfile;
    public bool activeWeapon = false;

    public override InteractionResult Interact()
    {
        if (activeWeapon)
        {
            return new InteractionResult(false, null, this.gameObject, InteractionType.None);
        }

        return new InteractionResult(true, weaponProfile, this.gameObject, InteractionType.Weapon);
    }
}
