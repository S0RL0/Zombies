using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Gun gun;                        // Reference to the gun
    public TextMeshProUGUI ammoText;       // Shows clip / reserve
    public TextMeshProUGUI reloadText;     // Shows "Reloading..."
    public Slider reloadBar;                // Shows reload progress

    private void Update()
    {
        if (!gun) return;

        if (gun.IsReloading)
        {
            // Show reload text
            if (reloadText)
                reloadText.text = "Reloading...";

            // Hide ammo text while reloading
            if (ammoText)
                ammoText.text = "";

            // Update reload bar
            if (reloadBar)
                reloadBar.value = gun.ReloadProgress; // 0 → 1
        }
        else
        {
            // Show ammo text
            if (ammoText)
                ammoText.text = $"{gun.CurrentClip} / {gun.ReserveAmmo}";

            // Hide reload text
            if (reloadText)
                reloadText.text = "";

            // Hide reload bar when not reloading
            if (reloadBar)
                reloadBar.value = 0;
        }
    }
}
