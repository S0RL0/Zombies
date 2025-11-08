using UnityEngine;

public class DamageableDummy : MonoBehaviour, IDamageable
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! Remaining HP: {health}");

        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}

