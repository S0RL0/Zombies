using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponProfile", menuName = "Weapons/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
    [Header("General Ammo & Fire")]
    [Tooltip("Number of bullets in a single clip.")]
    public int clipSize = 30;

    [Tooltip("Damage per bullet.")]
    public int damage = 10;

    [Tooltip("Rounds per minute for fire rate.")]
    public float fireRateRPM = 600f;

    [Tooltip("Time it takes to reload the weapon in seconds.")]
    public float reloadTime = 2f;

    [Tooltip("Number of shots in a burst (for burst fire).")]
    public int burstCount = 3;

    [Tooltip("Number of pellets fired per shot (for shotgun).")]
    public int pellets = 8;

    [Header("Projectile Settings")]
    [Tooltip("Projectile prefab used for projectile fire modes.")]
    public GameObject projectilePrefab;

    [Tooltip("Speed of the projectile in units per second.")]
    public float projectileSpeed = 30f;

    [Header("Shotgun Settings")]
    [Tooltip("Spread factor for shotgun pellets.")]
    public float spread = 0.05f;

    [Header("Recoil Settings")]
    [Tooltip("Vertical recoil applied per shot.")]
    public float verticalRecoil = 2f;

    [Tooltip("Maximum horizontal recoil applied per shot.")]
    public float horizontalRecoil = 1f;

    [Tooltip("Speed at which recoil recovers.")]
    public float recoilRecoverySpeed = 5f;

    [Header("Fire Mode")]
    [Tooltip("The type of fire mode this weapon uses.")]
    public FireModeType fireModeType = FireModeType.Hitscan;

    [Header("Effects")]
    [Tooltip("Muzzle flash particle system.")]
    public ParticleSystem muzzleFlash;

    [Tooltip("Impact effect prefab for hits.")]
    public GameObject impactEffect;

    [Tooltip("Sound played when firing.")]
    public AudioClip fireSound;

    [Tooltip("Sound played when reloading.")]
    public AudioClip reloadSound;

    [Header("Range & Physics")]
    [Tooltip("Maximum range of the weapon.")]
    public float range = 100f;
}


public enum FireModeType
{
    Hitscan,
    Projectile,
    Shotgun,
    Burst
}


