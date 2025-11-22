using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    // Serialized properties
    SerializedProperty rb, impactEffect, enemyLayer, projectileType;
    SerializedProperty damage;
    SerializedProperty bounciness, stuckDamage;
    SerializedProperty explosionDamage, minimumDamage, explosionRange, explosionForce;
    SerializedProperty maxCollisions, maxLifetime;

    void OnEnable()
    {
        // Universal references
        rb = serializedObject.FindProperty("rb");
        impactEffect = serializedObject.FindProperty("impactEffect");
        enemyLayer = serializedObject.FindProperty("enemyLayer");
        projectileType = serializedObject.FindProperty("projectileType");

        // Bullet stats
        damage = serializedObject.FindProperty("damage");

        // Stickybomb stats
        stuckDamage = serializedObject.FindProperty("stuckDamage");
        // stuckToEnemy intentionally hidden/private

        // Grenade stats
        bounciness = serializedObject.FindProperty("bounciness");
        maxCollisions = serializedObject.FindProperty("maxCollisions");
        maxLifetime = serializedObject.FindProperty("maxLifetime");

        // Explosion stats
        explosionDamage = serializedObject.FindProperty("explosionDamage");
        minimumDamage = serializedObject.FindProperty("minimumDamage");
        explosionRange = serializedObject.FindProperty("explosionRange");
        explosionForce = serializedObject.FindProperty("explosionForce");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- Projectile Type ---
        EditorGUILayout.LabelField("Projectile Type", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(projectileType);

        Projectile.ProjectileType type =
            (Projectile.ProjectileType)projectileType.enumValueIndex;

        // --- If None: hide all other fields ---
        if (type == Projectile.ProjectileType.None)
        {
            EditorGUILayout.HelpBox("Select a projectile type to view settings.", MessageType.Info);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.Space(10);

        // --- Universal References ---
        EditorGUILayout.LabelField("Universal References", EditorStyles.boldLabel);
        DrawSafe(rb);
        DrawSafe(impactEffect);
        DrawSafe(enemyLayer);

        EditorGUILayout.Space(10);

        // --- Type-Specific Fields ---
        switch (type)
        {
            case Projectile.ProjectileType.Bullet:
                EditorGUILayout.LabelField("Bullet Stats", EditorStyles.boldLabel);
                DrawSafe(damage);
                break;

            case Projectile.ProjectileType.StickyBomb:
                EditorGUILayout.LabelField("Stickybomb Stats", EditorStyles.boldLabel);
                DrawSafe(stuckDamage);
                // stuckToEnemy is intentionally hidden/private

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Explosion Stats", EditorStyles.boldLabel);
                DrawSafe(explosionDamage);
                DrawSafe(minimumDamage);
                DrawSafe(explosionRange);
                DrawSafe(explosionForce);
                break;

            case Projectile.ProjectileType.Rocket:
                EditorGUILayout.LabelField("Rocket Explosion Stats", EditorStyles.boldLabel);
                DrawSafe(explosionDamage);
                DrawSafe(minimumDamage);
                DrawSafe(explosionRange);
                DrawSafe(explosionForce);
                break;

            case Projectile.ProjectileType.Grenade:
                EditorGUILayout.LabelField("Grenade Stats", EditorStyles.boldLabel);
                DrawSafe(bounciness);      // only for Grenade
                DrawSafe(maxCollisions);
                DrawSafe(maxLifetime);

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Explosion Stats", EditorStyles.boldLabel);
                DrawSafe(explosionDamage);
                DrawSafe(minimumDamage);
                DrawSafe(explosionRange);
                DrawSafe(explosionForce);
                break;

            case Projectile.ProjectileType.ImpactGrenade:
                EditorGUILayout.LabelField("Impact Grenade Stats", EditorStyles.boldLabel);
                DrawSafe(maxCollisions);
                DrawSafe(maxLifetime);

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Explosion Stats", EditorStyles.boldLabel);
                DrawSafe(explosionDamage);
                DrawSafe(minimumDamage);
                DrawSafe(explosionRange);
                DrawSafe(explosionForce);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Prevents null-property crashes
    void DrawSafe(SerializedProperty prop)
    {
        if (prop != null)
            EditorGUILayout.PropertyField(prop);
    }
}
