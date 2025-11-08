using UnityEngine;

public static class FireModeFactory
{
    public static IFireMode Create(WeaponProfile profile, MonoBehaviour host = null)
    {
        switch (profile.fireModeType)
        {
            case FireModeType.Hitscan: return new HitscanMode();
            case FireModeType.Projectile: return new ProjectileMode();
            case FireModeType.Shotgun: return new ShotgunMode();
            case FireModeType.Burst: return new BurstMode(host);
            default: return new HitscanMode();
        }
    }
}

