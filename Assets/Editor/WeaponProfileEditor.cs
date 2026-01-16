using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponProfile))]
public class WeaponProfileEditor : Editor
{
    // Description
    SerializedProperty ID, nameProp, description, devDescription, icon, cost, ammoCost;
    SerializedProperty hitDetection, weaponType, fireMode, allowTriggerHold, prefab, upgradedProfile;

    // Firing stats
    SerializedProperty damage, rateOfFire, shotsPerBurst, timeBetweenBursts, spread, maxRange, rangeCurve;

    // Reload / ammo
    SerializedProperty reloadType, reloadTime, roundInsertTime, reloadWrapUpTime, magazineSize, TotalAmmo, ammoSize;

    // Projectile
    SerializedProperty projectilePrefab, forwardForce, upwardForce;

    // Sound
    SerializedProperty gunshotSFX, reloadSFX, roundInsertSFX, reloadWrapUpSFX, dryfireSFX;

    void OnEnable()
    {
        // Description
        ID = serializedObject.FindProperty("ID");
        nameProp = serializedObject.FindProperty("name");
        description = serializedObject.FindProperty("description");
        devDescription = serializedObject.FindProperty("devDescription");
        icon = serializedObject.FindProperty("icon");
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
        shotsPerBurst = serializedObject.FindProperty("shotsPerBurst");
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
        ammoSize = serializedObject.FindProperty("ammoSize");

        // Projectile
        projectilePrefab = serializedObject.FindProperty("projectilePrefab");
        forwardForce = serializedObject.FindProperty("forwardForce");
        upwardForce = serializedObject.FindProperty("upwardForce");

        // Sound
        gunshotSFX = serializedObject.FindProperty("gunshotSFX");
        reloadSFX = serializedObject.FindProperty("reloadSFX");
        roundInsertSFX = serializedObject.FindProperty("roundInsertSFX");
        reloadWrapUpSFX = serializedObject.FindProperty("reloadWrapUpSFX");
        dryfireSFX = serializedObject.FindProperty("dryfireSFX");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawWeaponDescription();
        EditorGUILayout.Space(8);
        DrawFiringStats();
        EditorGUILayout.Space(8);
        DrawReloadAndAmmo();     // includes ammoSize + auto-defaulting
        EditorGUILayout.Space(8);
        DrawProjectileStats();
        EditorGUILayout.Space(8);
        DrawSound();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawWeaponDescription()
    {
        EditorGUILayout.PropertyField(ID);
        EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(devDescription);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(cost);
        EditorGUILayout.PropertyField(ammoCost);

        EditorGUILayout.PropertyField(hitDetection);

        // Detect weaponType changes so we can default ammoSize
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(weaponType);
        bool weaponTypeChanged = EditorGUI.EndChangeCheck();

        EditorGUILayout.PropertyField(fireMode);
        EditorGUILayout.PropertyField(allowTriggerHold);

        EditorGUILayout.PropertyField(prefab);
        EditorGUILayout.PropertyField(upgradedProfile);

        if (weaponTypeChanged)
        {
            SetAmmoSizeDefaultFromWeaponType();
        }
    }

    void DrawFiringStats()
    {
        EditorGUILayout.PropertyField(damage);

        var fm = (FireMode)fireMode.enumValueIndex;

        if (fm == FireMode.Burst)
        {
            EditorGUILayout.PropertyField(rateOfFire, new GUIContent("Rate of Fire During Burst"));
            EditorGUILayout.PropertyField(timeBetweenBursts, new GUIContent("Time Between Bursts"));
            EditorGUILayout.PropertyField(shotsPerBurst, new GUIContent("Shots Per Burst"));
        }
        else
        {
            EditorGUILayout.PropertyField(rateOfFire, new GUIContent("Rate of Fire"));

            if (fm == FireMode.Scatter)
            {
                EditorGUILayout.PropertyField(shotsPerBurst, new GUIContent("Shots Per Scatter"));
            }
            // Singlefire & Authomatic intentionally hide shotsPerBurst and timeBetweenBursts
        }

        EditorGUILayout.PropertyField(spread);
        EditorGUILayout.PropertyField(maxRange);
        EditorGUILayout.PropertyField(rangeCurve);
    }

    void DrawReloadAndAmmo()
    {
        EditorGUILayout.PropertyField(reloadType);

        var rt = (ReloadType)reloadType.enumValueIndex;

        if (rt == ReloadType.Individual)
        {
            EditorGUILayout.PropertyField(roundInsertTime);
            EditorGUILayout.PropertyField(reloadWrapUpTime);
        }
        else
        {
            EditorGUILayout.PropertyField(reloadTime);
        }

        EditorGUILayout.PropertyField(magazineSize);
        EditorGUILayout.PropertyField(TotalAmmo);

        // ammoSize is always relevant, but gets auto-defaulted when weaponType changes
        EditorGUILayout.PropertyField(ammoSize, new GUIContent("Ammo Size"));
    }

    void DrawProjectileStats()
    {
        var hd = (HitDetectionModel)hitDetection.enumValueIndex;
        if (hd != HitDetectionModel.Projectile) return;

        EditorGUILayout.PropertyField(projectilePrefab);
        EditorGUILayout.PropertyField(forwardForce);
        EditorGUILayout.PropertyField(upwardForce);
    }

    void DrawSound()
    {
        EditorGUILayout.PropertyField(gunshotSFX);
        EditorGUILayout.PropertyField(dryfireSFX);


        var rt = (ReloadType)reloadType.enumValueIndex;

        if (rt == ReloadType.Individual)
        {
            EditorGUILayout.PropertyField(roundInsertSFX, new GUIContent("Round Insert SFX"));
            EditorGUILayout.PropertyField(reloadWrapUpSFX, new GUIContent("Wrap-Up Reload SFX"));
        }
        else
        {
            EditorGUILayout.PropertyField(reloadSFX, new GUIContent("Reload SFX"));
        }
    }

    void SetAmmoSizeDefaultFromWeaponType()
    {
        var wt = (WeaponType)weaponType.enumValueIndex;

        int targetAmmoSize = wt switch
        {
            WeaponType.Pistol => 1,
            WeaponType.SubmachineGun => 1,

            WeaponType.AssaultRifle => 2,
            WeaponType.DesignatedMarksmenRifle => 2,

            WeaponType.Sniper => 3,
            WeaponType.LightMachineGun => 3,

            WeaponType.Shotgun => 4,

            _ => ammoSize.intValue // don't change for Melee/unknown
        };

        if (targetAmmoSize != ammoSize.intValue)
        {
            ammoSize.intValue = targetAmmoSize;
        }
    }
}
