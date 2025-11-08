using UnityEngine;
using System.Collections;

public class BurstMode : IFireMode
{
    private MonoBehaviour coroutineHost;

    public BurstMode(MonoBehaviour host)
    {
        coroutineHost = host; // Needed to start coroutine
    }

    public void Fire(Vector3 origin, Vector3 direction, WeaponProfile profile, LayerMask hitMask)
    {
        if (coroutineHost != null)
            coroutineHost.StartCoroutine(FireBurst(origin, direction, profile, hitMask));
    }

    private IEnumerator FireBurst(Vector3 origin, Vector3 direction, WeaponProfile profile, LayerMask hitMask)
    {
        for (int i = 0; i < profile.burstCount; i++)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, profile.range, hitMask))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var target))
                    target.TakeDamage(profile.damage);

                if (profile.impactEffect)
                    Object.Instantiate(profile.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            yield return new WaitForSeconds(60f / profile.fireRateRPM); // spacing between burst shots
        }
    }
}

