using UnityEngine;

public class ParticleFX : MonoBehaviour
{
    public float lifetime = 0.5f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
