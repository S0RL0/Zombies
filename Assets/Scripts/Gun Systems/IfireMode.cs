using UnityEngine;

public interface IFireMode
{
    void Fire(Vector3 origin, Vector3 direction, OldWeaponProfile profile, LayerMask hitMask);

}
