using UnityEngine;

public class EmissionFlip : MonoBehaviour
{
    public Renderer targetRenderer;

    public Color emissionColorA = Color.red;
    public Color emissionColorB = Color.blue;

    public Light lightA;
    public Light lightB;

    [Range(0f, 10f)]
    public float emissionIntensity = 5f;

    public float speed = 2f;

    private Material materialInstance;

    void Start()
    {
        materialInstance = targetRenderer.material;
        materialInstance.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);

      
        Color baseColor = Color.Lerp(emissionColorA, emissionColorB, t);
        Color emission = baseColor * emissionIntensity;

        materialInstance.SetColor("_EmissionColor", emission);
        DynamicGI.SetEmissive(targetRenderer, emission);

       
        if (lightA != null) lightA.intensity = Mathf.Lerp(1f, 0f, t);
        if (lightB != null) lightB.intensity = Mathf.Lerp(0f, 1f, t);
    }
}
