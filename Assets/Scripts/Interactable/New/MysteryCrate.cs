using UnityEngine;

public class MysteryCrate : WeaponCrate
{
    [SerializeField] private GameObject lid;
    [SerializeField] private float openAngle = -180f;
    [SerializeField] private GameObject glow;
    private bool isOpen = false;
}
