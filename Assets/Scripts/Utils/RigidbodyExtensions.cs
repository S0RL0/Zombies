using UnityEngine;

public static class RigidbodyExtensions
{
    /// <summary>
    /// Toggle a Rigidbody's physics on or off.
    /// </summary>
    /// <param name="rb">The Rigidbody to toggle.</param>
    /// <param name="enabled">Whether physics should be enabled.</param>
    /// <returns>Returns the new enabled state, or null if Rigidbody is null.</returns>
    public static bool? ToggleRB(this Rigidbody rb, bool enabled)
    {
        if (rb == null) return null;

        if (!enabled) // turn physics OFF
        {
            //rb.linearVelocity = Vector3.zero;       
            //rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = false;
            return false;
        }
        else // turn physics ON
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.detectCollisions = true;
            return true;
        }
    }
}
