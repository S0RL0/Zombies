using UnityEngine;

public class Patriot : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem ps;
    public ParticleSystem ps2;

    [Header("Settings")]
    public float holdDuration = 10f;       // linger time
    public float transitionDuration = 10f; // fade time
    public float emissionIntensity = 2f;

    private Color[] colors = { Color.red, Color.white, Color.blue };
    private int currentColorIndex;
    private float timer;
    private bool isTransitioning = false;

    private Renderer[] renderers;
    private Material[] materials;

    void Start()
    {
        if (!ps)
            ps = GetComponentInChildren<ParticleSystem>();
        if (!ps2)
            ps2 = transform.Find("FlashHeadon").GetComponent<ParticleSystem>();

        // Get ALL renderers on this object and its children
        renderers = GetComponentsInChildren<Renderer>();

        // Cache material instances
        materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material; // instance material
            materials[i].EnableKeyword("_EMISSION");
        }

        ApplyColor(colors[0]);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isTransitioning)
        {
            // Lerp from current color to next color
            Color from = colors[currentColorIndex];
            Color to = colors[(currentColorIndex + 1) % colors.Length];
            float t = Mathf.Clamp01(timer / transitionDuration);
            Color current = Color.Lerp(from, to, t);

            ApplyColor(current);

            if (timer >= transitionDuration)
            {
                timer = 0f;
                isTransitioning = false;
                currentColorIndex = (currentColorIndex + 1) % colors.Length;
                ApplyColor(colors[currentColorIndex]); // ensure exact final color
            }
        }
        else
        {
            // Hold current color
            ApplyColor(colors[currentColorIndex]);

            if (timer >= holdDuration)
            {
                timer = 0f;
                isTransitioning = true;
            }
        }
    }

    void ApplyColor(Color color)
    {
        // Particle systems
        var main = ps.main;
        main.startColor = color;

        main = ps2.main;
        main.startColor = color;

        // All materials on all child renderers
        foreach (var mat in materials)
        {
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);

            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);

            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", color * emissionIntensity);
        }
    }
}
