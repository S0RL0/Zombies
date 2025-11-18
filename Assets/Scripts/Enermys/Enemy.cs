using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public GameObject floatingText;
    public virtual void TakeDamage(float amount)
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
