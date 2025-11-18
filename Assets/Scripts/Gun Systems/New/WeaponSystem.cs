using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    // Weapon description
    [Header("Desciptive")]
    public new string name;
    public string description;
    public string type; // Will be an enum when made into a SO

    // Weapon stats
    [Header("Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    // Projectile
    [Header("Projectile")]
    public GameObject projectile;
    public bool isProjectile; // Will be an enum when made into a SO
    public float shootForce, upwardForce;


    // Bools
    private bool shooting, readyToShoot, reloading;

    // Refernces
    [Header("References")]
    public Camera cam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask enemyLayer;

    // Graphics
    public GameObject muzzleFlash, bulletHit;
    //public CamShake camShake;
    public float camShakeMagnitude, camShakeDuraction;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        updateInput();

         updateUI();
    }

    private void updateInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Reload
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        // Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            if (isProjectile) ShootProjectile();
            else ShootRaycast();
        }
    }

    private void updateUI()
    {
        
    }

    private void ShootRaycast()
    {
        readyToShoot = false;

        // Spread
        float halfSpread = 0.5f * spread;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate direction with spread
        Vector3 direction = cam.transform.forward + new Vector3(x,y,0);

        // RayCast
        if (Physics.Raycast(cam.transform.position, direction, out rayHit, range))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                //rayHit.collider.GetComponent<Enemy>().TakeDamage(damage);
            }

        }



        // Camera Shake here

        // Particle effects here
        Instantiate(bulletHit, rayHit.point, Quaternion.Euler(0, 180, 0));
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        // Sound here

        // Adjust ammo
        bulletsLeft--;
        bulletsShot++;

        // Invoke resetShot
        Invoke("ResetShot", timeBetweenShooting);

        // Shoot again if more bulletsPerTap
        if (bulletsShot <= bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("ShootRaycast", timeBetweenShots);
        }
    }

    private void ShootProjectile()
    {
        readyToShoot = false;

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
        float halfSpread = 0.5f * spread;
        float x = Random.Range(-halfSpread, halfSpread);
        float y = Random.Range(-halfSpread, halfSpread);

        // Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate projectile
        GameObject currentProjectile = Instantiate(projectile, attackPoint.position, Quaternion.identity);

        // Rotate projectile to face the target
        currentProjectile.transform.forward = directionWithSpread.normalized;

        // Add forces to projectile
        currentProjectile.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentProjectile.GetComponent<Rigidbody>().AddForce(attackPoint.up * upwardForce, ForceMode.Impulse);


        // Adjust ammo
        bulletsLeft--;
        bulletsShot++;

        // Invoke resetShot
        Invoke("ResetShot", timeBetweenShooting);

        // Shoot again if more bulletsPerTap
        if (bulletsShot <= bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("ShootProjectile", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        // Animation here

        reloading = true;
        Invoke("ReloadFinish", reloadTime);
    }
    private void ReloadFinish()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
