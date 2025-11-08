using UnityEngine;

public class HitscanMode : IFireMode
{
    public void Fire(Vector3 origin, Vector3 direction, WeaponProfile profile, LayerMask hitMask)
    {
        Debug.DrawRay(origin, direction * profile.range, Color.red, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, profile.range, hitMask))
        {
            // Apply damage
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(profile.damage);

            // Impact effect
            if (profile.impactEffect)
                Object.Instantiate(profile.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}


