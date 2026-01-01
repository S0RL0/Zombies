using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static WeaponSystem;


public class WeaponSystem : MonoBehaviour
{
    public List<WeaponProfile> profiles;
    public List<int> bulletsInInventory;
    public List<GameObject> weaponModels;
    public int currentWeaponIndex = 0;
    public int inventorySize = 2;
    public Transform weaponPoint;

    int bulletsLeftInCurrentMag;
    int bulletsShotFromTriggerPull;

    #region Effects
    [Header("Effects")]
    public List<ParticleSystem> muzzleFlash;
    public GameObject bulletImpact;
    public List<ParticleSystem> caseEjection;
    #endregion

    #region Inputs and calculations
    // Input variables for input handler
    private bool fireKeyHeld;                // true while button is held
    private bool firePressedThisFrame;    // true only on the frame it was pressed
    private bool reloadPressedThisFrame;  // same for reload

    // Calculations
    private bool readyToFire;          // true when able to shoot
    private bool reloading;
    #endregion

    #region References
    // Refernces
    [Header("References")]
    public Camera cam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask enemyLayer;

    private EventInstance Reloadintance;
    [SerializeField] private EventReference ReloadEvent;

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

    #region Start and Update
    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();

        if (profiles.Count ==0)
        {
            Debug.LogError("WeaponSystem: No weapons assigned in the inspector.");
            return;
        }
            int i = 0;
        foreach (WeaponProfile w in profiles)
        {
            SetupWeapon(w, i, weaponModels[i].GetComponent<Weapon>());
            i++;
        }
        
    }

    private void SetupWeapon(WeaponProfile newWeapon, int newWeaponSlot, Weapon modelScript)
    {
        int currentInventorySize = profiles.Count;
        if (currentInventorySize < newWeaponSlot + 1)
        {
            profiles.Add(newWeapon);
            bulletsInInventory.Add(newWeapon.TotalAmmo);
            muzzleFlash.Add(modelScript.muzzleFlash);
            caseEjection.Add(modelScript.caseEjection);
        }
        else {
            bulletsInInventory[newWeaponSlot] = newWeapon.TotalAmmo;
            muzzleFlash[newWeaponSlot] = modelScript.muzzleFlash;
            caseEjection[newWeaponSlot] = modelScript.caseEjection;
        }

        bulletsLeftInCurrentMag = newWeapon.magazineSize;
        readyToFire = true;
    }

    private void Update()
    {
        if(profiles.Count == 0)
        {
            return;
        }
        updateInput();

        updateUI();

        UpdateGizmo();
    }

    private void updateInput()
    {
        // Decide if we should be shooting this frame
        bool shooting = profiles[currentWeaponIndex].allowTriggerHold ? fireKeyHeld : firePressedThisFrame;

        // Handle reload
        if (reloadPressedThisFrame && bulletsLeftInCurrentMag < profiles[currentWeaponIndex].magazineSize && !reloading)
        {
            Reload();
        }

        // Handle shooting
        if (readyToFire && shooting && !reloading && bulletsLeftInCurrentMag > 0)
        {
            bulletsShotFromTriggerPull = 0;

            if (profiles[currentWeaponIndex].hitDetection == HitDetectionModel.Projectile)
                ShootProjectile();
            else
                ShootRaycast();
        }

        // Reset one-frame flags
        firePressedThisFrame = false;
        reloadPressedThisFrame = false;
    }

    public void OnFirePerformed()
    {
        fireKeyHeld = true;
        firePressedThisFrame = true;  // one-frame flag
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
        float halfSpread = 0.5f * profiles[currentWeaponIndex].spread;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate direction with spread
        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

        // RayCast
        if (Physics.Raycast(cam.transform.position, direction, out rayHit, profiles[currentWeaponIndex].maxRange, enemyLayer))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<Enemy>().TakeDamage(profiles[currentWeaponIndex].damage);
            }

            CalculateGizmo(cam.transform.position, rayHit.point);
        }
        else
        {
            CalculateGizmo(cam.transform.position, cam.transform.position + (direction.normalized * profiles[currentWeaponIndex].maxRange));
        }

        // Camera Shake here

        // Particle effects here
        if (profiles[currentWeaponIndex].bulletImpact != null)
        {
            Quaternion rotation = Quaternion.LookRotation(rayHit.normal);
            Instantiate(profiles[currentWeaponIndex].bulletImpact, rayHit.point, rotation);
        }
        if (muzzleFlash[currentWeaponIndex] != null)
            muzzleFlash[currentWeaponIndex].Play();
        if (caseEjection[currentWeaponIndex] != null)
            caseEjection[currentWeaponIndex].Play();


        // Sound here

        // Adjust ammo
        bulletsLeftInCurrentMag--;
        bulletsShotFromTriggerPull++;

        // Shoot again if more shotsPerTriggerPull
        if (bulletsShotFromTriggerPull < profiles[currentWeaponIndex].shotsPerTriggerPull && bulletsLeftInCurrentMag > 0)
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
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        // Calculate spread
        float halfSpread = 0.5f * profiles[currentWeaponIndex].spread;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate projectile
        GameObject currentProjectile = Instantiate(profiles[currentWeaponIndex].projectilePrefab, attackPoint.position, Quaternion.identity);

        // Rotate projectile to face the target
        currentProjectile.transform.forward = directionWithSpread.normalized;

        // Add forces to projectile
        currentProjectile.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * profiles[currentWeaponIndex].forwardForce, ForceMode.Impulse);
        currentProjectile.GetComponent<Rigidbody>().AddForce(attackPoint.up * profiles[currentWeaponIndex].upwardForce, ForceMode.Impulse);


        // Adjust ammo
        bulletsLeftInCurrentMag--;
        bulletsShotFromTriggerPull++;

        // Invoke resetShot
        Invoke("ResetShot", profiles[currentWeaponIndex].timeBetweenBursts);

        // Shoot again if more shotsPerTriggerPull
        if (bulletsShotFromTriggerPull <= profiles[currentWeaponIndex].shotsPerTriggerPull && bulletsLeftInCurrentMag > 0)
        {
            Invoke("ShootProjectile", profiles[currentWeaponIndex].timeBetweenRounds);
        }
    }

    private void ResetShot()
    {
        readyToFire = true;
    }

    private void Reload()
    {
        Debug.Log("Reloading!");
        // Animation here
        // Sound Here

        //Reloadintance = RuntimeManager.CreateInstance(ReloadEvent);
       // Reloadintance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        //Reloadintance.start();
        //Reloadintance.release();

        reloading = true;

        Invoke("ReloadFinish", profiles[currentWeaponIndex].reloadTime);
    }

    private void ReloadFinish()
    {
        Debug.Log("Reloaded!");
        int bulletsNeeded = profiles[currentWeaponIndex].magazineSize - bulletsLeftInCurrentMag;
        if (bulletsInInventory[currentWeaponIndex] >= bulletsNeeded)
        {
            bulletsInInventory[currentWeaponIndex] -= bulletsNeeded;
        }
        else
        {
            bulletsNeeded = bulletsInInventory[currentWeaponIndex];
            bulletsInInventory[currentWeaponIndex] = 0;
        }
        bulletsLeftInCurrentMag += bulletsNeeded;

        reloading = false;    }
    #endregion

    #region Inventories and Weapon Switching
    private void SwitchToNextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= profiles.Count)
            currentWeaponIndex = 0;
    }

    private void SwitchToPreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = profiles.Count - 1;
    }

    public void PickupNewWeapon(WeaponProfile newWeapon, GameObject model, Weapon modelScript)
    {
        if (profiles.Count < inventorySize)
        {
            Debug.Log("Picked up weapon: " + newWeapon.name);
            // Add to Inventory
            //weapon.Add(newWeapon);
            SetupWeapon(newWeapon, currentWeaponIndex, modelScript);

            // Assign FX
            var systems = model.GetComponentsInChildren<ParticleSystem>();

            // Move weapon to hand
            model.transform.SetParent(weaponPoint);
            ToggleRB(model.GetComponent<Rigidbody>(), false);
            StartCoroutine(LerpRoutine(model, new Vector3(0,0,0), Quaternion.identity, 0.5f));

        }
        else
        {
            /*weapon[currentWeaponIndex] = newWeapon;
            GameObject droppedWeapon = Instantiate(weapon[currentWeaponIndex].weaponPrefab, transform.position + transform.forward, Quaternion.identity);
            droppedWeapon.GetComponent<Rigidbody>().AddForce(transform.forward * 2f, ForceMode.Impulse);
            droppedWeapon.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);*/
        }
    }

    public void MaxAmmo()
    {
        foreach (WeaponProfile w in profiles)
        {
            int ammoToAdd = w.TotalAmmo;
            int currentIndex = profiles.IndexOf(w);
            bulletsInInventory[currentIndex] = w.TotalAmmo;
            bulletsLeftInCurrentMag += w.magazineSize;
        }
    }

    public void RefillAmmo(WeaponProfile profile)
    {
        int currentIndex = profiles.IndexOf(profile);
        bulletsInInventory[currentIndex] = profile.TotalAmmo;
    }
    private IEnumerator LerpRoutine(GameObject obj, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Transform target = obj.transform;
        Vector3 startPos = target.localPosition;
        Quaternion startRot = target.localRotation;

        if (duration <= 0f)
        {
            target.localPosition = targetPosition;
            target.localRotation = targetRotation;
            yield break;
        }


        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            target.transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
            target.transform.localRotation = Quaternion.Slerp(startRot, targetRotation, t);

            yield return null;
        }

        target.localPosition = targetPosition;
        target.localRotation = targetRotation;
    }

    private bool? ToggleRB(Rigidbody rb, bool enabled)
    {
        if (rb == null) return null;

        if (!enabled) // turn physics OFF
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = false; // optional
            return enabled;
        }
        else // turn physics ON
        {
            rb.isKinematic = false;
            rb.useGravity = true; // set how you want when re-enabled
            rb.detectCollisions = true; // optional
            return true;
        }
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
        if(!showGizmos) return;

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
