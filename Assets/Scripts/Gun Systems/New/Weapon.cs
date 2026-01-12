using UnityEngine;

public class Weapon : Interactable
{
    public WeaponProfile weaponProfile;
    [HideInInspector] public bool activeWeapon = false;

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

    public void fireFX()
    {
        // Implement firing effects such as muzzle flash, sound, etc.
        if (muzzleFlash != null)
            muzzleFlash.Play();
        if (caseEjection != null)
            caseEjection.Play();
    }

    public override string GetInteactionText()
    {
        string str = "pick up " + weaponProfile.name;
        return str;
    }
}
