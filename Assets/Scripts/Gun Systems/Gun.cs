using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    [Header("Weapon Setup")]
    public WeaponProfile profile;
    public Transform muzzle;
    public LayerMask hitMask;

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;

    [Header("ADS Settings")]
    public Transform adsPosition;       // local target for ADS
    public float adsSpeed = 10f;        // interpolation speed
    public float adsFOV = 50f;          // optional FOV when aiming

    private IFireMode fireMode;
    private AudioSource audioSource;

    // Ammo
    private int currentClip;
    private int reserveAmmo;

    // Reload
    private bool isReloading;
    private float reloadTimer;

    // Recoil
    private Quaternion targetGunRotation;
    private Vector3 currentCameraRecoil;
    private Vector3 targetCameraRecoil;

    // Fire timing
    private float nextFireTime;

    // ADS state
    private bool isAiming;
    private Vector3 gunHipPosition;
    private float defaultFOV;

    // Input
    private PlayerControls controls;
    private bool isFiring;

    // --- Public getters for UI ---
    public int CurrentClip => currentClip;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;
    public float ReloadProgress => isReloading ? Mathf.Clamp01(reloadTimer / profile.reloadTime) : 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        fireMode = FireModeFactory.Create(profile, this);

        currentClip = profile.clipSize;
        reserveAmmo = 90;

        targetGunRotation = transform.localRotation;
        gunHipPosition = transform.localPosition;

        if (cameraTransform)
            defaultFOV = cameraTransform.GetComponent<Camera>().fieldOfView;

        // Input system
        controls = new PlayerControls();
        controls.Player.Fire.performed += ctx => isFiring = true;
        controls.Player.Fire.canceled += ctx => isFiring = false;
        controls.Player.Reload.performed += ctx => Reload();
        controls.Player.Aim.performed += ctx => isAiming = true;
        controls.Player.Aim.canceled += ctx => isAiming = false;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        if (!enabled) return; // stops input from affecting it
        HandleFiring();
        UpdateGunPositionAndRotation();
        RecoverCameraRecoil();
    }

    #region Firing

    private void HandleFiring()
    {
        if (isFiring)
            TryFire();
    }

    private void TryFire()
    {
        if (isReloading || Time.time < nextFireTime) return;

        if (currentClip <= 0)
        {
            Reload();
            return;
        }

        // Always use camera ray for direction
        Vector3 shootDirection = GetAimDirection();
        fireMode.Fire(muzzle.position, shootDirection, profile, hitMask);
        PlayEffects();

        currentClip--;

        // Recoil
        float x = Random.Range(-profile.horizontalRecoil, profile.horizontalRecoil);
        float y = profile.verticalRecoil;
        float recoilMultiplier = isAiming ? 0.5f : 1f;

        targetCameraRecoil += new Vector3(-y * recoilMultiplier, x * recoilMultiplier, 0);
        ApplyGunRecoil(y * recoilMultiplier, x * recoilMultiplier);

        nextFireTime = Time.time + (60f / profile.fireRateRPM);
    }

    private void PlayEffects()
    {
        if (profile.fireSound && audioSource)
            audioSource.PlayOneShot(profile.fireSound);

        if (profile.muzzleFlash)
            profile.muzzleFlash.Play();
    }

    #endregion

    #region ADS & Gun Rotation

    private Vector3 GetAimDirection()
    {
        if (!cameraTransform) return muzzle.forward;

        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, profile.range, hitMask))
            return (hit.point - muzzle.position).normalized;

        return ray.direction;
    }

    private void UpdateGunPositionAndRotation()
    {
        if (!cameraTransform) return;

        // Interpolate position toward ADS or hip
        Vector3 targetPos = isAiming && adsPosition != null ? adsPosition.localPosition : gunHipPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, adsSpeed * Time.deltaTime);

        // Rotation always follows camera + recoil
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraTransform.rotation * targetGunRotation, Time.deltaTime * 15f);

        // Optional: adjust FOV
        Camera cam = cameraTransform.GetComponent<Camera>();
        float targetFOV = isAiming ? adsFOV : defaultFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, adsSpeed * Time.deltaTime);
    }

    private void ApplyGunRecoil(float vertical, float horizontal)
    {
        Quaternion recoilOffset = Quaternion.Euler(-vertical * 0.5f, horizontal * 0.5f, 0);
        targetGunRotation *= recoilOffset;
    }

    private void RecoverCameraRecoil()
    {
        if (!cameraTransform) return;

        currentCameraRecoil = Vector3.Lerp(currentCameraRecoil, Vector3.zero, profile.recoilRecoverySpeed * Time.deltaTime);
        targetCameraRecoil = Vector3.Lerp(targetCameraRecoil, Vector3.zero, profile.recoilRecoverySpeed * Time.deltaTime);

        cameraTransform.localEulerAngles += targetCameraRecoil + currentCameraRecoil;
    }

    #endregion

    #region Reload

    public void Reload()
    {
        if (isReloading || currentClip == profile.clipSize || reserveAmmo <= 0) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        reloadTimer = 0f;

        if (profile.reloadSound && audioSource)
            audioSource.PlayOneShot(profile.reloadSound);

        if (animator)
            animator.SetTrigger("Reload");

        while (reloadTimer < profile.reloadTime)
        {
            reloadTimer += Time.deltaTime;
            yield return null;
        }

        int neededAmmo = profile.clipSize - currentClip;
        int ammoToLoad = Mathf.Min(neededAmmo, reserveAmmo);

        currentClip += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
        reloadTimer = 0f;
    }

    public void OnEquipped()
    {
        enabled = true; // now the gun responds to input
    }


    #endregion
}
