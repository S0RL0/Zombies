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
}
