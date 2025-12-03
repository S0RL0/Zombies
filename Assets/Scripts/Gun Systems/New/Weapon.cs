using UnityEngine;
using static UnityEditor.Progress;

public class Weapon : Interactable
{
    private WeaponProfile weaponProfile;

    public override InteractionResult Interact()
    {
        return new InteractionResult(true, gameObject);
    }
}
