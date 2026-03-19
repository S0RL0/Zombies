using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class WeaponSystem : MonoBehaviour
{
    #region Weapon System
    [Header("Weapon System")]
    public List<WeaponProfile> profiles;
    public List<int> bulletsInInventory;
    public List<GameObject> weaponModels;
    public List<Weapon> weaponScripts;
    public int currentWeaponIndex = 0;
    public int inventorySize = 2;
    public Transform weaponPoint;

    [SerializeField] private List<int> bulletsLeftInMag;
    private int bulletsShotFromTriggerPull;

    #endregion

    #region Recoil
    [Header("Recoil and ADS")]
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 naturalRotation = Vector3.zero;
    private bool isADS;
    [SerializeField] private bool recoilReturnCancelled = false;

    [SerializeField] private float recoilX = 2f;
    [SerializeField] private float recoilY = 2f;
    [SerializeField] private float recoilZ = 0.5f;

    [SerializeField] private float snappiness = 5f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private float cancelRecoilReturnThreshold = 0.05f;


    #endregion

    #region Effects
    [Header("Effects")]
    public List<GameObject> bulletImpact;
    #endregion

    #region Inputs and internal variables
    // Input variables for input handler
    private bool fireKeyHeld = false;                // true while button is held
    private bool firePressedThisFrame = false;    // true only on the frame it was pressed
    private bool reloadPressedThisFrame = false;  // same for reload


    // Calculations
    [SerializeField] private bool isFiring;             // true while firing (holding trigger for auto, or during burst)
    private bool readyToFire;          // true when able to shoot
    private bool reloading;
    private bool dryfire = true;

    #endregion

    #region References
    // Refernces
    [Header("References")]
    public PlayerController playerController;
    public Camera cam;
    public Transform recoilTransform;
    public Transform projectilePoint;
    public RaycastHit rayHit;
    public LayerMask enemyLayer;

    private EventInstance Reloadintance;
    [SerializeField] private EventReference ReloadEvent;

    private EventInstance GunshotInstance;
    private EventInstance DryInstance;
    #endregion

    #region Events
    // Events
    public static event Action<GameObject> onAmmoChanged;
    public static event Action<GameObject> onWeaponSwitched;
    public static event Action<GameObject> onInventoryChanged;

    #endregion

    #region Dev Tools
    [System.Serializable]
    public struct ShotGizmo     // Gizmo stuct
    {
        public Vector3 start;
        public Vector3 end;
        public float timeRemaining;

        public ShotGizmo(Vector3 start, Vector3 end, float duration)
        {
            this.start = start;
            this.end = end;
            this.timeRemaining = duration;
        }
    }
    [Header("Gizmo Settings")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float shotGizmoDuration = 1f;

    private List<ShotGizmo> shotGizmos = new List<ShotGizmo>();
    #endregion

    #region Start
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        cam = GetComponentInChildren<Camera>();
        recoilTransform = transform.Find("CameraRotation/CameraRecoil");

        // Create weapon models at start
        if (profiles.Count > 0)
        {
            foreach (WeaponProfile w in profiles)
            {
                GameObject model = Instantiate(w.prefab, weaponPoint.position, weaponPoint.rotation, weaponPoint);
                weaponModels.Add(model);
                model.GetComponent<Rigidbody>().ToggleRB(false);
            }
        }
        else
        {
            Debug.LogError("WeaponSystem: No weapons assigned in the inspector.");
            return;
        }
        // Assign variables for each weapon
        for (int i = 0; i < profiles.Count; i++)
        {
            if (i >= weaponModels.Count) break; // safety
            var model = weaponModels[i] ? weaponModels[i].GetComponent<Weapon>() : null;
            SetupWeapon(profiles[i], i, model);
        }
    }

    private void SetupWeapon(WeaponProfile newWeapon, int newWeaponSlot, Weapon modelScript)
    {
        if (newWeaponSlot < 0) return;

        // Only ensure the parallel lists, not profiles.
        EnsureSize(bulletsInInventory, newWeaponSlot + 1, 0);
        EnsureSize(weaponScripts, newWeaponSlot + 1, null);
        EnsureSize(bulletsLeftInMag, newWeaponSlot + 1, 0);

        bulletsInInventory[newWeaponSlot] = newWeapon != null ? newWeapon.TotalAmmo : 0;
        weaponScripts[newWeaponSlot] = modelScript != null ? modelScript : null;
        bulletsLeftInMag[newWeaponSlot] = newWeapon != null ? newWeapon.magazineSize : 0;
        readyToFire = true;
    }

    private static void EnsureSize<T>(List<T> list, int size, T filler)
    {
        while (list.Count < size) list.Add(filler);
    }
    #endregion

    #region Update
    private void Update()
    {
        if (profiles.Count == 0)
        {
            return;
        }
        UpdateInput();

        UpdateRecoil();

        updateUI();

        UpdateGizmo();
    }

    private void UpdateInput()
    {
        // Decide if we should be shooting this frame
        bool shooting = profiles[currentWeaponIndex].allowTriggerHold ? fireKeyHeld : firePressedThisFrame;
        isFiring = shooting && !reloading && bulletsLeftInMag[currentWeaponIndex] > 0;


        if (!isFiring && recoilReturnCancelled)
        {
            //ResetRecoil();
            // Reset override latch for next spray
            //recoilReturnCancelled = false;
        }

        // Handle reload
        if (reloadPressedThisFrame && bulletsLeftInMag[currentWeaponIndex] < profiles[currentWeaponIndex].magazineSize && !reloading)
        {
            Reload();
        }

        // Handle shooting
        if (readyToFire && shooting && !reloading && bulletsLeftInMag[currentWeaponIndex] > 0)
        {
            bulletsShotFromTriggerPull = 0;

            if (profiles[currentWeaponIndex].hitDetection == HitDetectionModel.Projectile)
                ShootProjectile();
            else
                ShootRaycast();
        }

        if (readyToFire && shooting && !reloading && bulletsLeftInMag[currentWeaponIndex] == 0 && dryfire)
        {

            DryInstance = RuntimeManager.CreateInstance(profiles[currentWeaponIndex].dryfireSFX);
            DryInstance.setVolume(1);
            DryInstance.start();
            DryInstance.release();
            dryfire = false;
            StartCoroutine(Timer());

        }

        // Reset one-frame flags
        firePressedThisFrame = false;
        reloadPressedThisFrame = false;
    }

    private void UpdateRecoil()
    {
        // Override if player looks while firing
        if (isFiring && !recoilReturnCancelled && playerController.lookMagnitude > cancelRecoilReturnThreshold)
        {
            recoilReturnCancelled = true;
        }

        Vector3 returnTarget = targetRotation;
        recoilReturnCancelled = false;
        // Always return Z (roll)
        returnTarget.z = Mathf.Lerp(targetRotation.z, 0f, Time.deltaTime * returnSpeed);

        // Only return X and Y if NOT overridden
        if (!recoilReturnCancelled)
        {
            returnTarget.x = Mathf.Lerp(targetRotation.x, naturalRotation.x, Time.deltaTime * returnSpeed);
            returnTarget.y = Mathf.Lerp(targetRotation.y, naturalRotation.y, Time.deltaTime * returnSpeed);
        }

        targetRotation = returnTarget;

        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.deltaTime * snappiness);
        recoilTransform.localRotation = Quaternion.Euler(currentRotation);
    }

    private void RecoilFire()
    {
        if (isADS)
            targetRotation += new Vector3(-recoilX * 0.5f, Random.Range(-recoilY * 0.5f, recoilY * 0.5f), Random.Range(-recoilZ * 0.5f, recoilZ * 0.5f));
        else
            targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
    private void ResetRecoil()
    {
        naturalRotation = recoilTransform.localRotation.eulerAngles;
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        dryfire = true;
    }

    public void OnFirePerformed()
    {
        fireKeyHeld = true;
        firePressedThisFrame = true;  // one-frame flag

        //recoilReturnCancelled = false;
        //ResetRecoil();
    }

    public void OnFireCanceled()
    {
        fireKeyHeld = false;
    }

    public void OnReloadPerformed()
    {
        reloadPressedThisFrame = true;
    }

    private void updateUI()
    {

    }
    #endregion

    #region Shooting and Reloading
    private void ShootRaycast()
    {
        readyToFire = false;

        // Spread
        float halfSpread = 0.5f * profiles[currentWeaponIndex].spread * 0.001f;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate direction with spread
        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

        // RayCast
        if (Physics.Raycast(cam.transform.position, direction, out rayHit, profiles[currentWeaponIndex].maxRange, enemyLayer))
        {
            Enemy enemy = rayHit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                if (rayHit.collider.name == "Head")
                    enemy.TakeDamage(profiles[currentWeaponIndex].damage * profiles[currentWeaponIndex].headshotMultiplier);
                else
                    enemy.TakeDamage(profiles[currentWeaponIndex].damage);
            }

            if (rayHit.collider.GetComponent<Destructable>() != null)
                rayHit.collider.GetComponent<Destructable>().TakeDamage(profiles[currentWeaponIndex].damage);

            CalculateGizmo(cam.transform.position, rayHit.point);
        }
        else
        {
            CalculateGizmo(cam.transform.position, cam.transform.position + (direction.normalized * profiles[currentWeaponIndex].maxRange));
        }

        // Recoil
        RecoilFire();

        // Camera Shake here


        // Particle effects here
        weaponModels[currentWeaponIndex].GetComponent<Weapon>().fireFX();

        if (bulletImpact.Count > 0)
        {
            string impactTag = rayHit.collider != null ? rayHit.collider.tag : "Untagged";
            int impactIndex = 0;
            switch (impactTag)
            {
                case "Stone":
                    impactIndex = 0;
                    break;
                case "Metal":
                    impactIndex = 1;
                    break;
                case "Wood":
                    impactIndex = 2;
                    break;
                case "Enemy":
                    impactIndex = 3;
                    break;
            }

            if (rayHit.collider != null && rayHit.normal != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(rayHit.normal);
                Instantiate(bulletImpact[impactIndex], rayHit.point, rotation, rayHit.collider.transform);
            }
        }
        // int fXVolume = AudioSettingsManager.Instance.FXVolume;

        GunshotInstance = RuntimeManager.CreateInstance(profiles[currentWeaponIndex].gunshotSFX);
        GunshotInstance.setVolume(0.2F);
        GunshotInstance.start();
        GunshotInstance.release();

        // Adjust ammo
        bulletsLeftInMag[currentWeaponIndex]--;
        bulletsShotFromTriggerPull++;
        onAmmoChanged?.Invoke(gameObject);

        // Shoot again if more shotsPerTriggerPull
        if (bulletsShotFromTriggerPull < profiles[currentWeaponIndex].shotsPerBurst && bulletsLeftInMag[currentWeaponIndex] > 0)
        {
            Invoke("ShootRaycast", profiles[currentWeaponIndex].timeBetweenRounds);
            return;
        }

        // Invoke resetShot
        if (profiles[currentWeaponIndex].fireMode == FireMode.Burst)
        {
            Invoke("ResetShot", profiles[currentWeaponIndex].timeBetweenBursts);
        }
        else if (profiles[currentWeaponIndex].fireMode == FireMode.Singlefire || profiles[currentWeaponIndex].fireMode == FireMode.Authomatic)
        {
            Invoke("ResetShot", profiles[currentWeaponIndex].timeBetweenRounds);
        }
    }

    private void ShootProjectile()
    {
        readyToFire = false;

        // Find the hit position using raycast
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Check if the ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75); // Arbitrary distance if nothing is hit
        }

        // Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - projectilePoint.position;

        // Calculate spread
        float halfSpread = 0.5f * profiles[currentWeaponIndex].spread;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate projectile
        GameObject currentProjectile = Instantiate(profiles[currentWeaponIndex].projectilePrefab, projectilePoint.position, Quaternion.identity);

        // Rotate projectile to face the target
        currentProjectile.transform.forward = directionWithSpread.normalized;

        // Add forces to projectile
        currentProjectile.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * profiles[currentWeaponIndex].forwardForce, ForceMode.Impulse);
        currentProjectile.GetComponent<Rigidbody>().AddForce(projectilePoint.up * profiles[currentWeaponIndex].upwardForce, ForceMode.Impulse);


        // Adjust ammo
        bulletsLeftInMag[currentWeaponIndex]--;
        bulletsShotFromTriggerPull++;
        onAmmoChanged?.Invoke(gameObject);

        // Invoke resetShot
        Invoke("ResetShot", profiles[currentWeaponIndex].timeBetweenBursts);

        // Shoot again if more shotsPerTriggerPull
        if (bulletsShotFromTriggerPull <= profiles[currentWeaponIndex].shotsPerBurst && bulletsLeftInMag[currentWeaponIndex] > 0)
        {
            Invoke("ShootProjectile", profiles[currentWeaponIndex].timeBetweenRounds);
        }
    }

    private void ResetShot()
    {
        readyToFire = true;
    }
    #endregion

    #region Reloading
    private void Reload()
    {
        // Animation here
        // Sound Here

        Reloadintance = RuntimeManager.CreateInstance(profiles[currentWeaponIndex].reloadSFX);
        Reloadintance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        Reloadintance.start();
        Reloadintance.release();

        reloading = true;

        Invoke("ReloadFinish", profiles[currentWeaponIndex].reloadTime);
    }

    private void ReloadFinish()
    {
        int bulletsNeeded = profiles[currentWeaponIndex].magazineSize - bulletsLeftInMag[currentWeaponIndex];
        if (bulletsInInventory[currentWeaponIndex] >= bulletsNeeded)
        {
            bulletsInInventory[currentWeaponIndex] -= bulletsNeeded;
        }
        else
        {
            bulletsNeeded = bulletsInInventory[currentWeaponIndex];
            bulletsInInventory[currentWeaponIndex] = 0;
        }
        bulletsLeftInMag[currentWeaponIndex] += bulletsNeeded;

        onAmmoChanged?.Invoke(gameObject);
        reloading = false;
    }

    private IEnumerator ReloadInventory(int index, float delay)
    {
        yield return new WaitForSeconds(delay);

        int bulletsNeeded = profiles[index].magazineSize - bulletsLeftInMag[index];
        if (bulletsInInventory[index] >= bulletsNeeded)
        {
            bulletsInInventory[index] -= bulletsNeeded;
        }
        else
        {
            bulletsNeeded = bulletsInInventory[index];
            bulletsInInventory[index] = 0;
        }
        bulletsLeftInMag[index] += bulletsNeeded;
    }
    #endregion

    #region Inventories and Weapon Switching
    private void SwitchToNextWeapon()
    {
        if (profiles.Count == 0) return;

        int previousIndex = currentWeaponIndex;
        if (previousIndex < profiles.Count)
            StartCoroutine(ReloadInventory(previousIndex, profiles[previousIndex].reloadTime));

        currentWeaponIndex = (currentWeaponIndex + 1) % profiles.Count;

        if (previousIndex < profiles.Count)
            SetWeaponActive(previousIndex, false);
        SetWeaponActive(currentWeaponIndex, true);

        // Trigger event
        onWeaponSwitched?.Invoke(gameObject);

    }

    private void SwitchToPreviousWeapon()
    {
        if (profiles.Count == 0) return;

        int previousIndex = currentWeaponIndex;
        StartCoroutine(ReloadInventory(previousIndex, profiles[previousIndex].reloadTime));

        // Add Count before modulo to avoid negative values
        currentWeaponIndex = (currentWeaponIndex - 1 + profiles.Count) % profiles.Count;

        SetWeaponActive(previousIndex, false);
        SetWeaponActive(currentWeaponIndex, true);

        // Trigger event
        onWeaponSwitched?.Invoke(gameObject);
    }

    private void SetWeaponActive(int index, bool active)
    {
        if (index >= 0 && index < weaponModels.Count)
            weaponModels[index].SetActive(active);
    }

    public void PickupNewWeapon(WeaponProfile newWeapon, GameObject model, Weapon modelScript)
    {
        if (profiles.Count < inventorySize)
        {



            // Add to Inventory
            profiles.Add(newWeapon);
            weaponModels.Add(model);
            bulletsLeftInMag.Add(newWeapon.magazineSize);
            SwitchToNextWeapon();

            SetupWeapon(newWeapon, currentWeaponIndex, modelScript);

            // Move weapon to hand
            model.transform.SetParent(weaponPoint);
            model.GetComponent<Rigidbody>().ToggleRB(false);
            TweenUtils.LerpTween(model, Vector3.zero, Quaternion.identity, 0.5f, Ease.OutCubic);

            // Trigger event
            onInventoryChanged?.Invoke(gameObject);
            Debug.Log("added: Inventory size: " + inventorySize + " |current weapon count: " + weaponModels.Count);

        }
        else
        {

            DropCurrentWeapon(true);

            // Add new weapon to inventory
            profiles.Add(newWeapon);
            weaponModels.Add(model);
            bulletsLeftInMag.Add(newWeapon.magazineSize);
            SwitchToNextWeapon();
            SetupWeapon(newWeapon, currentWeaponIndex, modelScript);

            Debug.Log("Replaced weapon with: " + newWeapon.name);

            // Move weapon to hand
            model.transform.SetParent(weaponPoint);
            model.GetComponent<Rigidbody>().ToggleRB(false);
            TweenUtils.LerpTween(model, Vector3.zero, Quaternion.identity, 0.5f, Ease.OutCubic);



            // Trigger event
            onInventoryChanged?.Invoke(gameObject);
            Debug.Log("replaced: Inventory size: " + inventorySize + " |current weapon count: " + weaponModels.Count);

        }
    }

    public GameObject DropCurrentWeapon(bool applyPhysics)
    {
        // Drop current weapon and remove from inventory
        if (profiles.Count == 0) return null;
        Debug.Log("Dropped weapon: " + profiles[currentWeaponIndex].name);
        GameObject droppedmodel = weaponModels[currentWeaponIndex];
        weaponModels.RemoveAt(currentWeaponIndex);
        droppedmodel.transform.SetParent(null);
        droppedmodel.GetComponent<Rigidbody>().ToggleRB(false);
        if (applyPhysics)
        {
            droppedmodel.GetComponent<Rigidbody>().ToggleRB(true);
            droppedmodel.GetComponent<Rigidbody>().AddForce(transform.forward * 2f, ForceMode.Impulse);
            droppedmodel.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
        profiles.RemoveAt(currentWeaponIndex);
        bulletsInInventory.RemoveAt(currentWeaponIndex);
        weaponScripts.RemoveAt(currentWeaponIndex);
        bulletsLeftInMag.RemoveAt(currentWeaponIndex);
        SwitchToNextWeapon();
        return droppedmodel;
    }

    public WeaponProfile GetCurrentWeaponProfile()
    {
        if (profiles.Count == 0) return null;
        return profiles[currentWeaponIndex];
    }

    public void MaxAmmo()
    {
        foreach (WeaponProfile w in profiles)
        {
            int ammoToAdd = w.TotalAmmo;
            int currentIndex = profiles.IndexOf(w);
            bulletsInInventory[currentIndex] = w.TotalAmmo;
            bulletsLeftInMag[currentIndex] += w.magazineSize;

            // Trigger event
            onAmmoChanged?.Invoke(gameObject);
        }
    }

    public void RefillAmmo(WeaponProfile profile)
    {
        int currentIndex = profiles.IndexOf(profile);
        bulletsInInventory[currentIndex] = profile.TotalAmmo;

        // Trigger event
        onAmmoChanged?.Invoke(gameObject);
    }

    public List<int> GetAmmoCount(int index)
    {
        if (index < 0 || index >= profiles.Count || index >= bulletsLeftInMag.Count)
        {
            return new List<int> { 0, 0 };
        }

        List<int> ammoCount = new List<int>();
        if (index == currentWeaponIndex)
        {
            ammoCount.Add(bulletsLeftInMag[index]);
        }
        else
        {
            ammoCount.Add(profiles[index].magazineSize);
        }
        if (index >= 0 && index < bulletsInInventory.Count)
        {
            ammoCount.Add(bulletsInInventory[index]);

        }
        else
        {
            ammoCount.Add(0);
        }
        return ammoCount;
    }

    public List<int> GetAmmoCount(WeaponProfile profile)
    {
        int index = profiles.IndexOf(profile);
        if (index < 0 || index >= profiles.Count)
        {
            return new List<int> { 0, 0 };
        }

        List<int> ammoCount = new List<int>();
        ammoCount.Add(bulletsLeftInMag[index]);
        ammoCount.Add(bulletsInInventory[index]);
        return ammoCount;
    }

    #endregion

    #region Dev tools
    private void CalculateGizmo(Vector3 position, Vector3 point)
    {
        shotGizmos.Add(new ShotGizmo(position, point, shotGizmoDuration));
    }

    private void UpdateGizmo()
    {
        if (!showGizmos) return;
        for (int i = shotGizmos.Count - 1; i >= 0; i--)
        {
            ShotGizmo gizmo = shotGizmos[i];
            gizmo.timeRemaining -= Time.deltaTime;

            if (gizmo.timeRemaining <= 0f)
            {
                shotGizmos.RemoveAt(i);
            }
            else
            {
                shotGizmos[i] = gizmo;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.red;

        foreach (var g in shotGizmos)
        {
            Gizmos.DrawLine(g.start, g.end);
            Gizmos.DrawSphere(g.start, 0.03f);
            Gizmos.DrawSphere(g.end, 0.06f);
        }
    }
    #endregion
}