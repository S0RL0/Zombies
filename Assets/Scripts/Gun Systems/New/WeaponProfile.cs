using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponProfile", menuName = "Weapons/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
    #region Weapon Description
    [Header("Weapon description")]
    [Tooltip("Unique ID")]
    public int ID;
    [Tooltip("Weapon Name")]
    public new string name;
    [Tooltip("Weapon Description")]
    public string description;
    [Tooltip("Developer Description (not shown in game)")]
    public string devDescription;
    [Tooltip("Weapon Icon")]
    public Sprite icon;
    [Tooltip("Cost to buy weapon")]
    public int cost;
    [Tooltip("Cost to buy ammo")]
    public int ammoCost;
    [Tooltip("Weapon hit detection model either Raycast or Projectile.")]
    public HitDetectionModel hitDetection;
    [Tooltip("Weapon Catagorie e.g. Assault Rifle, Sniper, Shotgun")]
    public WeaponType weaponType;
    [Tooltip("Weapon firemode, how many bullets are fired and how")]
    public FireMode fireMode;
    [Tooltip("Allow holding down the trigger to keep firing.")]
    public bool allowTriggerHold;
    [Tooltip("Weapon prefab")]
    public GameObject prefab;
    [Tooltip("Upgraded weapon profile")]
    public WeaponProfile upgradedProfile;
    #endregion

    #region Weapon Stats    
    [Header("Weapon Firing Stats")]
    [Tooltip("Damage per bullet.")]
    public int damage;
    [Tooltip("Rate of fire in Rounds per minute")]
    public float rateOfFire;
    [HideInInspector]public float timeBetweenRounds;
    [Tooltip("Number of bullets fired per trigger pull.")]
    public int shotsPerBurst = 1;
    [Tooltip("Time between bursts.")]
    public float timeBetweenBursts;
    [Tooltip("Bullet spread.")]
    public float spread;
    [Tooltip("Max range of gun.")]
    public float maxRange;
    [Tooltip("Damage over range cure.")]
    public AnimationCurve rangeCurve;
    #endregion

    #region Reloading and Ammo
    [Header("Weapon Reloading and Ammo")]
    [Tooltip("How the gun is loaded.")]
    public ReloadType reloadType;
    [Tooltip("Time taken for reload animation.")]
    public float reloadTime;
    [Tooltip("Time taken between individual bullet loads (for individual reload types).")]
    public float roundInsertTime;
    [Tooltip("Time taken to finish reloading after individual bullet loads")]
    public float reloadWrapUpTime;
    [Tooltip("Number of bullets in a single clip.")]
    public int magazineSize;
    [Tooltip("Total ammo carried by player for this weapon.")]
    public int TotalAmmo;
    [Tooltip("Ammo size (1-4). 1: Small, 2: Medium, 3: Large, 4: Shotgun")]
    [Range(1,4)]
    public int ammoSize;

    #endregion

    #region Projectile Stats
    [Header("Projection Stats")]
    [Tooltip("Prefab of projectile game object.")]
    public GameObject projectilePrefab;
    [Tooltip("Forward force applied to the projectile.")]
    public float forwardForce;
    [Tooltip("Upward force applied to the projectile, for arced projectiles.")]
    public float upwardForce;
    #endregion

    #region Weapon Handling
    [Header("Weapon Handling, Sway & Recoil")]
    #endregion

    #region Sound
    [Header("Sound")]
    [Tooltip("Gunshot sound effect")]
    public EventReference gunshotSFX;
    [Tooltip("Reload sound effect")]
    public EventReference reloadSFX;
    [Tooltip("Individual reload Sound effect for individual reload")]
    public EventReference roundInsertSFX;
    [Tooltip("Wrapup reload Sound effect for individual reload")]
    public EventReference reloadWrapUpSFX;
    [Tooltip("dryfire sound effect")]
    public EventReference dryfireSFX;


    #endregion


    private void OnValidate()
    {
        timeBetweenRounds = 60f / rateOfFire;
    }
}
public enum HitDetectionModel
{
    Raycast,
    Projectile
}
public enum WeaponType
{
    Melee,
    Pistol,
    SubmachineGun,
    AssaultRifle,
    DesignatedMarksmenRifle,
    Shotgun,
    Sniper,
    LightMachineGun
}
public enum FireMode
{
    Singlefire,
    Authomatic,
    Burst,
    Scatter // For shotguns
}
public enum ReloadType
{
    Magazine,
    Individual,
    Manual // Requires overload function, e.g. bolt action
}


