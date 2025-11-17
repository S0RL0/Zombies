using UnityEngine;

public class DamageableDummy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public GameObject floatingText;

    void Start()
    {
        TakeDamage(20);
    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        GameObject Text = Instantiate(floatingText, transform.position, Quaternion.identity);
        if (Text.GetComponent<FloatingText>())
            Text.GetComponent<FloatingText>().SetText(amount.ToString(), false);

        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}

