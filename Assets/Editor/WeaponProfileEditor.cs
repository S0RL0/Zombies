using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponProfile))]
public class WeaponProfileEditor : Editor
{
    // Description
    SerializedProperty ID, nameProp, description, devDescription, cost, ammoCost;
    SerializedProperty hitDetection, weaponType, fireMode, allowTriggerHold, prefab, upgradedProfile;

    // Firing stats
    SerializedProperty damage, rateOfFire, shotsPerTriggerPull, timeBetweenBursts, spread, maxRange, rangeCurve;

    // Reload / ammo
    SerializedProperty reloadType, reloadTime, roundInsertTime, reloadWrapUpTime, magazineSize, TotalAmmo;

    // Projectile
    SerializedProperty projectilePrefab, forwardForce, upwardForce;

    // Effects
    SerializedProperty bulletImpact;

    // Sound
    SerializedProperty gunshotSFX, reloadSFX, roundInsertSFX, reloadWrapUpSFX;

    void OnEnable()
    {
        // Description
        ID = serializedObject.FindProperty("ID");
        nameProp = serializedObject.FindProperty("name");
        description = serializedObject.FindProperty("description");
        devDescription = serializedObject.FindProperty("devDescription");
        cost = serializedObject.FindProperty("cost");
        ammoCost = serializedObject.FindProperty("ammoCost");
        hitDetection = serializedObject.FindProperty("hitDetection");
        weaponType = serializedObject.FindProperty("weaponType");
        fireMode = serializedObject.FindProperty("fireMode");
        allowTriggerHold = serializedObject.FindProperty("allowTriggerHold");
        prefab = serializedObject.FindProperty("prefab");
        upgradedProfile = serializedObject.FindProperty("upgradedProfile");

        // Firing
        damage = serializedObject.FindProperty("damage");
        rateOfFire = serializedObject.FindProperty("rateOfFire");
        shotsPerTriggerPull = serializedObject.FindProperty("shotsPerTriggerPull");
        timeBetweenBursts = serializedObject.FindProperty("timeBetweenBursts");
        spread = serializedObject.FindProperty("spread");
        maxRange = serializedObject.FindProperty("maxRange");
        rangeCurve = serializedObject.FindProperty("rangeCurve");

        // Reload / ammo
        reloadType = serializedObject.FindProperty("reloadType");
        reloadTime = serializedObject.FindProperty("reloadTime");
        roundInsertTime = serializedObject.FindProperty("roundInsertTime");
        reloadWrapUpTime = serializedObject.FindProperty("reloadWrapUpTime");
        magazineSize = serializedObject.FindProperty("magazineSize");
        TotalAmmo = serializedObject.FindProperty("TotalAmmo");

        // Projectile
        projectilePrefab = serializedObject.FindProperty("projectilePrefab");
        forwardForce = serializedObject.FindProperty("forwardForce");
        upwardForce = serializedObject.FindProperty("upwardForce");

        // Effects
        bulletImpact = serializedObject.FindProperty("bulletImpact");

        // Sound
        gunshotSFX = serializedObject.FindProperty("gunshotSFX");
        reloadSFX = serializedObject.FindProperty("reloadSFX");
        roundInsertSFX = serializedObject.FindProperty("roundInsertSFX");
        reloadWrapUpSFX = serializedObject.FindProperty("reloadWrapUpSFX");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawWeaponDescription();
        EditorGUILayout.Space(8);
        DrawFiringStats();
        EditorGUILayout.Space(8);
        DrawReloadAndAmmo();
        EditorGUILayout.Space(8);
        DrawProjectileStats();
        EditorGUILayout.Space(8);
        DrawEffectsAndSound();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawWeaponDescription()
    {
        EditorGUILayout.PropertyField(ID);
        EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(devDescription);
        EditorGUILayout.PropertyField(cost);
        EditorGUILayout.PropertyField(ammoCost);

        EditorGUILayout.PropertyField(hitDetection);
        EditorGUILayout.PropertyField(weaponType);
        EditorGUILayout.PropertyField(fireMode);
        EditorGUILayout.PropertyField(allowTriggerHold);

        EditorGUILayout.PropertyField(prefab);
        EditorGUILayout.PropertyField(upgradedProfile);
    }

    void DrawFiringStats()
    {

        EditorGUILayout.PropertyField(damage);

        var fm = (FireMode)fireMode.enumValueIndex;

        // FireMode rules:
        // - Singlefire or Automatic: hide timeBetweenBursts and shotsPerTriggerPull
        // - Burst: show both, rename RPM and shots field
        // - Scatter: hide timeBetweenBursts, rename shotsPerTriggerPull to pellets per shot

        if (fm == FireMode.Burst)
        {
            EditorGUILayout.PropertyField(rateOfFire, new GUIContent("Rate of Fire During Burst"));
            EditorGUILayout.PropertyField(timeBetweenBursts, new GUIContent("Time Between Bursts"));
            EditorGUILayout.PropertyField(shotsPerTriggerPull, new GUIContent("Shots Per Burst"));
        }
        else
        {
            // Normal RPM label unless Burst (your requirement only changes it for Burst)
            EditorGUILayout.PropertyField(rateOfFire, new GUIContent("Rate of Fire"));

            if (fm == FireMode.Scatter)
            {
                // Scatter: show shots but rename, hide timeBetweenBursts
                EditorGUILayout.PropertyField(shotsPerTriggerPull, new GUIContent("Pellets Per Shot"));
            }
            else
            {
                // Singlefire or Authomatic: hide both
                // (Also covers any other non-burst/non-scatter modes you may add later.)
                if (fm != FireMode.Singlefire && fm != FireMode.Authomatic)
                {
                    // If you ever add a mode where you want it visible, it’ll show here by default
                    EditorGUILayout.PropertyField(shotsPerTriggerPull, new GUIContent("Shots Per Trigger Pull"));
                    EditorGUILayout.PropertyField(timeBetweenBursts, new GUIContent("Time Between Bursts"));
                }
            }
        }

        EditorGUILayout.PropertyField(spread);
        EditorGUILayout.PropertyField(maxRange);
        EditorGUILayout.PropertyField(rangeCurve);
    }

    void DrawReloadAndAmmo()
    {

        EditorGUILayout.PropertyField(reloadType);

        var rt = (ReloadType)reloadType.enumValueIndex;

        // Reload rules:
        // - Magazine: hide roundInsertTime, reloadWrapUpTime and their SFX
        // - Individual: hide reloadTime and reloadSFX; show insert/wrap times + SFX
        // - Manual: not specified; sensible default = treat like Magazine (uses reloadTime/reloadSFX)
        if (rt == ReloadType.Individual)
        {
            EditorGUILayout.PropertyField(roundInsertTime, new GUIContent("Round Insert Time"));
            EditorGUILayout.PropertyField(reloadWrapUpTime, new GUIContent("Reload Wrap-Up Time"));
        }
        else
        {
            EditorGUILayout.PropertyField(reloadTime, new GUIContent("Reload Time"));
        }

        EditorGUILayout.PropertyField(magazineSize);
        EditorGUILayout.PropertyField(TotalAmmo);
    }

    void DrawProjectileStats()
    {
        // Hit detection rule:
        // When hit detection is NOT Projectile, hide all projectile related variables.
        var hd = (HitDetectionModel)hitDetection.enumValueIndex;
        if (hd != HitDetectionModel.Projectile) return;

        EditorGUILayout.PropertyField(projectilePrefab);
        EditorGUILayout.PropertyField(forwardForce);
        EditorGUILayout.PropertyField(upwardForce);
    }

    void DrawEffectsAndSound()
    {
        EditorGUILayout.PropertyField(bulletImpact);

        EditorGUILayout.Space(6);

        EditorGUILayout.PropertyField(gunshotSFX);

        var rt = (ReloadType)reloadType.enumValueIndex;

        if (rt == ReloadType.Individual)
        {
            // Individual: hide reloadSFX; show per-step SFX
            EditorGUILayout.PropertyField(roundInsertSFX, new GUIContent("Round Insert SFX"));
            EditorGUILayout.PropertyField(reloadWrapUpSFX, new GUIContent("Wrap-Up Reload SFX"));
        }
        else
        {
            // Magazine (and Manual by default): show reloadSFX; hide per-step SFX
            EditorGUILayout.PropertyField(reloadSFX, new GUIContent("Reload SFX"));
        }
    }
}
