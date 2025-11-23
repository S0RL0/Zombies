using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light targetLight;

    [Header("Flicker Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f;   // How quickly the light changes
    public bool useRandomSpeed = true;  // Optional randomness

    private float baseIntensity;

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        baseIntensity = targetLight.intensity;
    }

    void Update()
    {
        float speed = useRandomSpeed ? Random.Range(flickerSpeed * 0.5f, flickerSpeed * 1.5f) : flickerSpeed;

        float newIntensity = Mathf.Lerp(
            targetLight.intensity,
            Random.Range(minIntensity, maxIntensity),
            speed
        );

        targetLight.intensity = newIntensity;
    }
}

