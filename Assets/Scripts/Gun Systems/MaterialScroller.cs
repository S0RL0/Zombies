using UnityEngine;

public class MaterialScroller : MonoBehaviour
{
    public float scrollSpeedX = 0.01f;
    public float scrollSpeedY = 0.0f;

    private Vector2 offset;

    void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        // Apply to this object and all children
        ApplyOffsetRecursively(transform, offset);
    }

    void ApplyOffsetRecursively(Transform parent, Vector2 offset)
    {
        foreach (Transform child in parent)
        {
            Renderer rend = child.GetComponent<Renderer>();
            if (rend != null && rend.sharedMaterial != null)
            {
                rend.sharedMaterial.mainTextureOffset = offset;
            }

            // Recursive call for grandchildren
            if (child.childCount > 0)
            {
                ApplyOffsetRecursively(child, offset);
            }
        }

        // Also apply to the parent itself
        Renderer parentRend = parent.GetComponent<Renderer>();
        if (parentRend != null && parentRend.sharedMaterial != null)
        {
            parentRend.sharedMaterial.mainTextureOffset = offset;
        }
    }
}
