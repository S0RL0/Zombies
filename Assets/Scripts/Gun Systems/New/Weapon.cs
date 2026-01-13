using UnityEngine;

public class Weapon : Interactable
{
    public WeaponProfile profile;
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

        return new InteractionResult(true, profile, this.gameObject, InteractionType.Weapon);
    }

    public void fireFX()
    {
        // Implement firing effects such as muzzle flash, sound, etc.
        if (muzzleFlash != null)
            muzzleFlash.Play();
        if (caseEjection != null)
            caseEjection.Play();
    }

    public override string GetInteractionText()
    {
        string str = "to Pick Up " + profile.name;
        return str;
    }

    public override Sprite GetInteractionIcon()
    {
        return profile.icon;
    }
}
