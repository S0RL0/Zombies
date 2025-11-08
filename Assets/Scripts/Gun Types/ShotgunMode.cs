using UnityEngine;

public class ShotgunMode : IFireMode
{
    public void Fire(Vector3 origin, Vector3 direction, WeaponProfile profile, LayerMask hitMask)
    {
        for (int i = 0; i < profile.pellets; i++)
        {
            // Spread
            Vector3 spread = direction + new Vector3(
                Random.Range(-profile.spread, profile.spread),
                Random.Range(-profile.spread, profile.spread),
                Random.Range(-profile.spread, profile.spread)
            );

            if (Physics.Raycast(origin, spread.normalized, out RaycastHit hit, profile.range, hitMask))
            {
                Debug.DrawRay(origin, direction * profile.range, Color.red, 1f);

                if (hit.collider.TryGetComponent<IDamageable>(out var target))
                    target.TakeDamage(profile.damage / profile.pellets);

                if (profile.impactEffect)
                    Object.Instantiate(profile.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}


