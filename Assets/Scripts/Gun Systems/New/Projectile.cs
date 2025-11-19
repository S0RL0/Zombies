using System.Text;
using UnityEngine;



public class Projectile : MonoBehaviour
{
    public enum ProjectileType
    {
        None,
        Bullet,
        Grenade,
        ImpactGrenade,
        Rocket,
        StickyBomb
    }

    public ProjectileType projectileType;

    // Universal references
    private Rigidbody rb;
    public GameObject impactEffect;
    public LayerMask enemyLayer;

    //Universal Stats
    private bool useGravity;

    // Bullet Stats 
    public float damage;

    // Grenade Stats
    [Range(0f, 1f)]
    public float bounciness = 0f;

    // Stickybomb Stats
    public float stuckDamage;
    

    // Rocket/Grenade/Stickybomb Stats
    public float explosionDamage;
    public float minimumDamage;
    public float explosionRange;
    public float explosionForce;

    // Grendade Stats
    public int maxCollisions = 1;
    public float maxLifetime = 10f;

    // Private variables
    private bool stuckToEnemy = false;
    private int collisions = 0;
    private PhysicsMaterial physicsMaterial;

    private void Awake()
    {
        if (projectileType == ProjectileType.None)
        {
            Debug.LogWarning("Projectile type is set to None. Please select a valid projectile type.");
            return;
        }
        else if (projectileType == ProjectileType.Bullet || projectileType == ProjectileType.Rocket)
        {
            useGravity = false;

        }
        else
        {
            useGravity = true;
        }
        // Sets the rigidbody gravity
        rb.useGravity = useGravity;

        Setup();
    }

    private void Update()
    {
        // Countdown to explosion
        maxLifetime -= Time.deltaTime;

        // If lifetime is up explode or destroy
        if (maxLifetime <= 0)
        {
            if (projectileType != ProjectileType.Bullet)
            {
                if (maxLifetime <= 0)
                {
                    Explode();
                }
            }
            else
            {
                Invoke("DelayedDestroy", 0.05f);
            }
        }

        // If the projectile has exceeded max collisions explode
        if (collisions >= maxCollisions && projectileType == ProjectileType.Grenade)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;
        if (projectileType == ProjectileType.Bullet)
        {
            Enemy script = collision.collider.GetComponent<Enemy>();
            if (script != null)
            {
                script.TakeDamage(damage);
            }
            ;
        }
        else if (projectileType == ProjectileType.Rocket)
        {
            Explode();
        }
        else if (projectileType == ProjectileType.ImpactGrenade)
        {
            // Explode on impact with enemy
            if (collision.collider.CompareTag("Enemy"))
            {
                Explode();

            }
        }
        else if (projectileType == ProjectileType.Grenade)
        {
            if (collisions >= maxCollisions)
            {
                Explode();
            }
        }
    }

    private void Explode()
    {
        // Instantiate explosion
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Check for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, enemyLayer);

        for (int i = 0;  i < enemies.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, enemies[i].transform.position);
            float calculatedDamage = explosionDamage * 1 - (distance / explosionRange);
            if(calculatedDamage < minimumDamage)
            {
                calculatedDamage = minimumDamage;
            }

            Enemy script = enemies[i].GetComponent<Enemy>();
            if (script != null)
            {
                script.TakeDamage(calculatedDamage);
            }
        }

        Invoke("DelayedDestroy", 0.05f);
    }

    private void DelayedDestroy()
    {
        Destroy(gameObject);
    }

    private void Setup()
    {
        // Create a new physic material
        physicsMaterial = new PhysicsMaterial();
        physicsMaterial.bounciness = bounciness;
        physicsMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
        physicsMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;

        //Assign material to collider
        GetComponent<SphereCollider>().material = physicsMaterial;

        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
