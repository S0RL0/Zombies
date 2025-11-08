using UnityEngine;

public class PlayerGunManager : MonoBehaviour
{
    public Transform gunHolder; // assign GunHolder
    public GameObject gunPrefab;

    private GameObject equippedGun;

    private void Start()
    {
        EquipGun(gunPrefab);
    }

    public void EquipGun(GameObject gun)
    {
        if (equippedGun) Destroy(equippedGun);
        equippedGun = Instantiate(gun, gunHolder);
        equippedGun.transform.localPosition = Vector3.zero;
        equippedGun.transform.localRotation = Quaternion.identity;
    }
}
