using System.Collections;
using UnityEngine;

public static class Utils
{
    public static bool? ToggleRB(Rigidbody rb, bool enabled)
    {
        if (rb == null) return null;

        if (!enabled) // turn physics OFF
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = false; // optional
            return enabled;
        }
        else // turn physics ON
        {
            rb.isKinematic = false;
            rb.useGravity = true; // set how you want when re-enabled
            rb.detectCollisions = true; // optional
            return true;
        }
    }
    private static IEnumerator LerpRoutine(GameObject obj, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Transform target = obj.transform;
        Vector3 startPos = target.localPosition;
        Quaternion startRot = target.localRotation;

        if (duration <= 0f)
        {
            target.localPosition = targetPosition;
            target.localRotation = targetRotation;
            yield break;
        }


        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            target.transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
            target.transform.localRotation = Quaternion.Slerp(startRot, targetRotation, t);

            yield return null;
        }

        target.localPosition = targetPosition;
        target.localRotation = targetRotation;
    }
}
