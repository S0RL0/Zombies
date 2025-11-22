using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f;
    public float moveSpeed = 1f;
    public Vector2 randomDirectionRange = new Vector2(-0.5f, 0.5f);

    private float timer = 0f;
    private Vector3 moveDirection;
    private TMP_Text tmp;
    private Color startColor;

    void Start()
    {
        tmp = GetComponentInChildren<TMP_Text>();
        if (tmp == null)
        {
            Debug.LogError("FloatingText requires a TMP_Text component.");
            return;
        }

        startColor = tmp.color;

        // Create a slightly random upward direction
        moveDirection = new Vector3(
            Random.Range(randomDirectionRange.x, randomDirectionRange.y),
            1f,
            Random.Range(randomDirectionRange.x, randomDirectionRange.y)
        ).normalized;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Movement
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Face camera
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform.position);

        // Fade out
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // Destroy
        if (timer >= lifetime)
            Destroy(gameObject);
    }


    public void SetText(string message, bool highlight)
    {
        if (tmp == null)
            tmp = GetComponentInChildren<TMP_Text>();

        if (highlight)
        {
            tmp.color = Color.red;
            startColor = tmp.color;
        }
            

        tmp.text = message;
    }
}
