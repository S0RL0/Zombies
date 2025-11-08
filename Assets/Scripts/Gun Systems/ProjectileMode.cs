
using UnityEngine;

public class ProjectileMode : IFireMode
{
    public void Fire(Vector3 origin, Vector3 direction, WeaponProfile profile, LayerMask hitMask)
    {
        if (!profile.projectilePrefab) return;

        GameObject projectile = Object.Instantiate(profile.projectilePrefab, origin, Quaternion.LookRotation(direction));
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * profile.projectileSpeed; // Projectile moves along aim direction
    }
}



