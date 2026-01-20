using System.Collections.Generic;
using UnityEngine;

public class MaterialScroller : MonoBehaviour
{
    public float scrollSpeedX = 0.01f;
    public float scrollSpeedY = 0.01f;

    private Vector2 offset;

    private List<Renderer> renderers = new List<Renderer>();

    private void Start()
    {
        // Initialize the list of renderers from this object and all children
        CollectRenderersRecursively(transform);

    }

    void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        // Apply to this object and all children
        ApplyOffset(offset);
    }

    void ApplyOffset(Vector2 offset)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend != null && rend.sharedMaterial != null)
            {
                rend.sharedMaterial.mainTextureOffset = offset;
            }
        }
    }

    void CollectRenderersRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Renderer rend = child.GetComponent<Renderer>();

            // Exclude renders with particle systems
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (rend != null && ps == null)
            {
                renderers.Add(rend);
            }
            // Recursive call for grandchildren
            if (child.childCount > 0)
            {
                CollectRenderersRecursively(child);
            }
        }
        // Also add the parent itself
        Renderer parentRend = parent.GetComponent<Renderer>();
        if (parentRend != null)
        {
            renderers.Add(parentRend);
        }
    }
}
