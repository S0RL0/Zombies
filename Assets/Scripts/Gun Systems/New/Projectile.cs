using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 50f;
    public float lifetime = 5f;
    Rigidbody rb;

    void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        CancelInvoke();
        Invoke(nameof(Despawn), lifetime);
    }

    void OnCollisionEnter(Collision col)
    {
        var dmg = col.collider.GetComponent<IDamageable>();
        if (dmg != null) dmg.TakeDamage(damage);
        // TODO: impact VFX / sound / splash damage
        Despawn();
    }

    void Despawn()
    {
        gameObject.SetActive(false);
    }
}
