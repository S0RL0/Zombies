using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    // Weapon description
    public new string name;
    public string description, type;

    // Weapon stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    // Bools
    private bool shooting, readyToShoot, reloading;

    // Refernces
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
            Shoot();
        }
    }

    private void updateUI()
    {
        
    }

    private void Shoot()
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

        bulletsLeft--;
        bulletsShot++;
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot <= bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("finishReload", reloadTime);
    }
    private void finishReload()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
