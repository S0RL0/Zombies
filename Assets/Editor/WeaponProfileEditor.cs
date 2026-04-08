using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponProfile))]
public class WeaponProfileEditor : Editor
{
    // Description
    SerializedProperty ID, nameProp, description, devDescription, icon, cost, ammoCost;
    SerializedProperty hitDetection, weaponType, fireMode, allowTriggerHold, prefab, upgradedProfile;

    // Firing stats
    SerializedProperty damage, headshotMultiplier, rateOfFire, shotsPerBurst, timeBetweenBursts, spread, maxRange, rangeCurve;

    // Recoil
    SerializedProperty hasRandomizedVerticalRecoil, hasRandomizedHorizontalRecoil;
    SerializedProperty verticalRecoil, horizontalRecoil, verticalRecoilADS, horizontalRecoilADS;

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
        headshotMultiplier = serializedObject.FindProperty("headshotMultiplier");
        rateOfFire = serializedObject.FindProperty("rateOfFire");
        shotsPerBurst = serializedObject.FindProperty("shotsPerBurst");
        timeBetweenBursts = serializedObject.FindProperty("timeBetweenBursts");
        spread = serializedObject.FindProperty("spread");
        maxRange = serializedObject.FindProperty("maxRange");
        rangeCurve = serializedObject.FindProperty("rangeCurve");

        // Recoil
        hasRandomizedVerticalRecoil = serializedObject.FindProperty("hasRandomizedVerticalRecoil");
        hasRandomizedHorizontalRecoil = serializedObject.FindProperty("hasRandomizedHorizontalRecoil");
        verticalRecoil = serializedObject.FindProperty("verticalRecoil");
        horizontalRecoil = serializedObject.FindProperty("horizontalRecoil");
        verticalRecoilADS = serializedObject.FindProperty("verticalRecoilADS");
        horizontalRecoilADS = serializedObject.FindProperty("horizontalRecoilADS");

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
        Space();
        DrawFiringStats();
        Space();
        DrawRecoil();
        Space();
        DrawReloadAndAmmo();
        Space();
        DrawProjectileStats();
        Space();
        DrawSound();

        serializedObject.ApplyModifiedProperties();
    }

    void Space() => EditorGUILayout.Space(10);

    void DrawWeaponDescription()
    {
        EditorGUILayout.LabelField("Weapon Description", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(ID);
        EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(devDescription);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(cost);
        EditorGUILayout.PropertyField(ammoCost);
        EditorGUILayout.PropertyField(hitDetection);

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
        EditorGUILayout.LabelField("Firing Stats", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(headshotMultiplier);

        var fm = (FireMode)fireMode.enumValueIndex;

        if (fm == FireMode.Burst)
        {
            EditorGUILayout.PropertyField(rateOfFire, new GUIContent("Rate of Fire During Burst"));
            EditorGUILayout.PropertyField(timeBetweenBursts);
            EditorGUILayout.PropertyField(shotsPerBurst);
        }
        else
        {
            EditorGUILayout.PropertyField(rateOfFire);

            if (fm == FireMode.Scatter)
            {
                EditorGUILayout.PropertyField(shotsPerBurst, new GUIContent("Pellet Count"));
            }
        }

        EditorGUILayout.PropertyField(spread);
        EditorGUILayout.PropertyField(maxRange);
        EditorGUILayout.PropertyField(rangeCurve);
    }

    void DrawRecoil()
    {
        EditorGUILayout.LabelField("Recoil", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(hasRandomizedVerticalRecoil);
        EditorGUILayout.PropertyField(hasRandomizedHorizontalRecoil);

        EditorGUILayout.PropertyField(verticalRecoil);
        EditorGUILayout.PropertyField(horizontalRecoil);

        EditorGUILayout.PropertyField(verticalRecoilADS);
        EditorGUILayout.PropertyField(horizontalRecoilADS);
    }

    void DrawReloadAndAmmo()
    {
        EditorGUILayout.LabelField("Reload & Ammo", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(reloadType);

        var rt = (ReloadType)reloadType.enumValueIndex;

        switch (rt)
        {
            case ReloadType.Individual:
                EditorGUILayout.PropertyField(roundInsertTime);
                EditorGUILayout.PropertyField(reloadWrapUpTime);
                break;

            case ReloadType.Magazine:
                EditorGUILayout.PropertyField(reloadTime);
                break;

            case ReloadType.Manual:
                EditorGUILayout.HelpBox("Manual reload requires custom scripting (e.g. bolt-action).", MessageType.Info);
                break;
        }

        EditorGUILayout.PropertyField(magazineSize);
        EditorGUILayout.PropertyField(TotalAmmo);
        EditorGUILayout.PropertyField(ammoSize);
    }

    void DrawProjectileStats()
    {
        var hd = (HitDetectionModel)hitDetection.enumValueIndex;
        if (hd != HitDetectionModel.Projectile) return;

        EditorGUILayout.LabelField("Projectile", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(projectilePrefab);
        EditorGUILayout.PropertyField(forwardForce);
        EditorGUILayout.PropertyField(upwardForce);
    }

    void DrawSound()
    {
        EditorGUILayout.LabelField("Sound", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(gunshotSFX);
        EditorGUILayout.PropertyField(dryfireSFX);

        var rt = (ReloadType)reloadType.enumValueIndex;

        if (rt == ReloadType.Individual)
        {
            EditorGUILayout.PropertyField(roundInsertSFX);
            EditorGUILayout.PropertyField(reloadWrapUpSFX);
        }
        else
        {
            EditorGUILayout.PropertyField(reloadSFX);
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
            _ => ammoSize.intValue
        };

        if (targetAmmoSize != ammoSize.intValue)
        {
            ammoSize.intValue = targetAmmoSize;
        }
    }
}