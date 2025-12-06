using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponProfile", menuName = "Weapons/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
    #region Weapon Description
    [Header("Weapon description")]
    [Tooltip("Weapon Name")]
    public new string name;
    [Tooltip("Weapon Description")]
    public string description;
    [Tooltip("Weapon hit detection model either Raycast or Projectile.")]
    public HitDetectionModel hitDetection;
    [Tooltip("Weapon Catagorie e.g. Assault Rifle, Sniper, Shotgun")]
    public WeaponType weaponType;
    [Tooltip("Weapon firemode, how many bullets are fired and how")]
    public FireMode fireMode;
    [Tooltip("Weapon prefab")]
    public GameObject weaponPrefab;
    #endregion


    #region Weapon Stats    
    [Header("Weapon Firing Stats")]
    [Tooltip("Damage per bullet.")]
    public int damage;
    [Tooltip("Rounds per minute for fire rate.")]
    public float roundsPerMinute = 600f;
    public float timeBetweenRounds;
    [Tooltip("Number of bullets fired per trigger pull.")]
    public int shotsPerTriggerPull = 1;
    [Tooltip("Time between trigger pulls.")]
    public float timeBetweenTriggerPull;
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
    public int IntialAmmo;
    [Tooltip("Allow holding down the trigger to keep firing.")]
    public bool allowTriggerHold;
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

    #region Effects
    [Header("Effects")]
    [Tooltip("Muzzle Flash FX Game Object for when gun fired")]
    public GameObject muzzleFlash;
    [Tooltip("Muzzle Flash FX Origin Point")]
    public Transform muzzleFlashOrigin;

    [Tooltip("Bullet Impact FX Game Object for when bullet hits")]
    public GameObject bulletImpact;

    [Tooltip("Case Ejection FX Game Object for when gun fired")]
    public GameObject caseEjection;
    [Tooltip("Case Ejection FX Origin Point")]
    public Transform caseEjectionOrigin;

    public float camShakeMagnitude;
    public float camShakeDuraction;
    #endregion

    /*
    #region Sound
    [Header("Sound")]
    #endregion
    */

    private void OnValidate()
    {
        timeBetweenRounds = 60f / roundsPerMinute;
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


