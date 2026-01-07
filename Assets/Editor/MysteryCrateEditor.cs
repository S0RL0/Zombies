using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MysteryCrate))]
public class MysteryCrateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the Script field as read-only (like Unity normally does)
        GUI.enabled = false;
        EditorGUILayout.ObjectField(
            "Script",
            MonoScript.FromMonoBehaviour((MysteryCrate)target),
            typeof(MysteryCrate),
            false
        );
        GUI.enabled = true;

        // Draw everything except the inherited "profile" field
        DrawPropertiesExcluding(serializedObject, "profile", "m_Script");

        serializedObject.ApplyModifiedProperties();
    }
}