using UnityEngine;

public interface IFireMode
{
    void Fire(Vector3 origin, Vector3 direction, WeaponProfile profile, LayerMask hitMask);

}
