using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponProfile", menuName = "Weapons/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
    [Header("Weapon description")]
    [Tooltip("Weapon Name")]
    public new string name;
    [Tooltip("Weapon Description")]
    public string description;
    [Tooltip("Weapon firemode")]
    public FireMode fireMode;
    [Tooltip("Weapon prefab")]
    public GameObject weaponPrefab;

    [Header("Weapon Firing Stats")]
    [Tooltip("Damage per bullet.")]
    public int damage;
    [Tooltip("Rounds per minute for fire rate.")]
    public float roundsPerMinute = 600f;
    [Tooltip("Number of bullets fired per trigger pull.")]
    public int shotsPerTriggerPull = 1;
    [Tooltip("Time before first shot fired.")]
    public float initialShotDelay;
    [Tooltip("Bullet spread.")]
    public float spread;
    [Tooltip("Max range of gun.")]
    public float maxRange;
    [Tooltip("Damage over range cure.")]
    public AnimationCurve rangeCurve;

    [Header("Weapon Reloading and Ammo")]
    [Tooltip("How the gun is loaded.")]
    public ReloadType reloadType;
    [Tooltip("Time taken for reload animation.")]
    public float reloadTime;
    [Tooltip("Time taken between individual bullet loads (for individual reload types).")]
    public float roundInsertTime;
    [Tooltip("Number of bullets in a single clip.")]
    public int magazineSize;
    [Tooltip("Allow holding down the trigger to keep firing.")]
    public bool allowButtonHold;

    [Header("Projection Stats")]
    [Tooltip("Prefab of projectile game object.")]
    public GameObject projectilePrefab;
    [Tooltip("Forward force applied to the projectile.")]
    public float forwardForce;
    [Tooltip("Upward force applied to the projectile, for arced projectiles.")]
    public float upwardForce;



    [Header("Weapon Handling, Sway & Recoil")]


    [Header("Firing FX")]
    public GameObject muzzleFlash;
    public GameObject bulletImpact;

    //[Header("Sound")]

}

public enum FireMode
{
    Singlefire,
    Authomatic,
    Burst,
    Shotgun,
    Projectile
}
public enum ReloadType
{
    Magazine,
    Individual,
    Manual // Requires overload function, e.g. bolt action
}


