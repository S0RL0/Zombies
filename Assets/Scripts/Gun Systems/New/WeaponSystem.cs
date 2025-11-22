using System.Collections.Generic;
using UnityEngine;
using static WeaponSystem;


public class WeaponSystem : MonoBehaviour
{

    public WeaponProfile weapon;

    #region Inputs and calculations
    // Input variables for input handler
    private bool fireKeyHeld;                // true while button is held
    private bool firePressedThisFrame;    // true only on the frame it was pressed
    private bool reloadPressedThisFrame;  // same for reload

    // Calculations
    private bool readyToFire;          // true when able to shoot
    int bulletsLeft;
    int bulletsShot;
    private bool reloading;
    #endregion

    #region References
    // Refernces
    [Header("References")]
    public Camera cam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask enemyLayer;
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


    private void Awake()
    {
        bulletsLeft = weapon.magazineSize;
        readyToFire = true;
        cam = GetComponentInParent<Camera>();
    }

    private void Update()
    {
        updateInput();

        updateUI();

        UpdateGizmo();
    }

    private void updateInput()
    {
        // Decide if we should be shooting this frame
        bool shooting = weapon.allowTriggerHold ? fireKeyHeld : firePressedThisFrame;

        // Handle reload
        if (reloadPressedThisFrame && bulletsLeft < weapon.magazineSize && !reloading)
        {
            Reload();
        }

        // Handle shooting
        if (readyToFire && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            if (weapon.hitDetection == HitDetectionModel.Projectile)
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

    private void ShootRaycast()
    {
        readyToFire = false;

        // Spread
        float halfSpread = 0.5f * weapon.spread;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate direction with spread
        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

        // RayCast
        if (Physics.Raycast(cam.transform.position, direction, out rayHit, weapon.maxRange))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<Enemy>().TakeDamage(weapon.damage);
            }

            CalculateGizmo(cam.transform.position, rayHit.point);
        }
        else
        {
            CalculateGizmo(cam.transform.position, cam.transform.position + (direction.normalized * weapon.maxRange));
        }

        // Camera Shake here

        // Particle effects here
        if (weapon.bulletImpact != null)
            Instantiate(weapon.bulletImpact, rayHit.point, Quaternion.Euler(0, 180, 0));
        if (weapon.muzzleFlash != null)
            Instantiate(weapon.muzzleFlash, attackPoint.position, Quaternion.identity);


        // Sound here

        // Adjust ammo
        bulletsLeft--;
        bulletsShot++;

        // Invoke resetShot
        Invoke("ResetShot", weapon.timeBetweenTriggerPull);

        // Shoot again if more shotsPerTriggerPull
        if (bulletsShot <= weapon.shotsPerTriggerPull && bulletsLeft > 0)
        {
            Invoke("ShootRaycast", weapon.timeBetweenRounds);
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
        float halfSpread = 0.5f * weapon.spread;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate projectile
        GameObject currentProjectile = Instantiate(weapon.projectilePrefab, attackPoint.position, Quaternion.identity);

        // Rotate projectile to face the target
        currentProjectile.transform.forward = directionWithSpread.normalized;

        // Add forces to projectile
        currentProjectile.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * weapon.forwardForce, ForceMode.Impulse);
        currentProjectile.GetComponent<Rigidbody>().AddForce(attackPoint.up * weapon.upwardForce, ForceMode.Impulse);


        // Adjust ammo
        bulletsLeft--;
        bulletsShot++;

        // Invoke resetShot
        Invoke("ResetShot", weapon.timeBetweenTriggerPull);

        // Shoot again if more shotsPerTriggerPull
        if (bulletsShot <= weapon.shotsPerTriggerPull && bulletsLeft > 0)
        {
            Invoke("ShootProjectile", weapon.timeBetweenRounds);
        }
    }

    private void ResetShot()
    {
        readyToFire = true;
    }

    private void Reload()
    {
        // Animation here

        reloading = true;
        Invoke("ReloadFinish", weapon.reloadTime);
    }
    private void ReloadFinish()
    {
        bulletsLeft = weapon.magazineSize;
        reloading = false;
    }

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
}
