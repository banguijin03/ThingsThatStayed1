using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UI_InputBlocker))]
public class UI_InputBlockerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UI_InputBlocker blocker = (UI_InputBlocker)target;

        serializedObject.Update();

        SerializedProperty allowedInput =
            serializedObject.FindProperty("allowedInput");

        allowedInput.intValue = (int)(InputType)EditorGUILayout.EnumFlagsField(
            "Allowed Input",
            (InputType)allowedInput.intValue
        );

        serializedObject.ApplyModifiedProperties();
    }
}