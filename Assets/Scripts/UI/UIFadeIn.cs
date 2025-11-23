using System.Collections;
using UnityEngine;

public class UIFadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float duration = 1f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float time = 0f;
        canvasGroup.alpha = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}

