using UnityEngine;

public class Weapon : Interactable
{
    public WeaponProfile weaponProfile;
    [HideInInspector]public bool activeWeapon = false;

    // Effects
    public ParticleSystem muzzleFlash;
    public ParticleSystem caseEjection;

    public override InteractionResult Interact()
    {
        if (activeWeapon)
        {
            return new InteractionResult(false, null, this.gameObject, InteractionType.None);
        }

        return new InteractionResult(true, weaponProfile, this.gameObject, InteractionType.Weapon);
    }

    public void fireEffects()
    {
        // Implement firing effects such as muzzle flash, sound, etc.
        muzzleFlash.Play();
        caseEjection.Play();
    }
}
