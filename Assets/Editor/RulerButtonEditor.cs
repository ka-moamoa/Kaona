using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MokuSelectorGameManager))]
public class MokuSelectorGameManagerEditor : Editor
{
    private readonly string[] sectionLabels = { "FF", "WS", "FE", "PB", "TM", "SS" };
    private readonly string[] materialNames = { "Deactivated", "Unhealed", "Healed" };
    private bool[] foldouts = new bool[6];

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw top-level animator
        SerializedProperty rulerPanelAnim = serializedObject.FindProperty("RulerPanelAnim");
        EditorGUILayout.PropertyField(rulerPanelAnim, new GUIContent("Ruler Panel Animator"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Moku Properties", EditorStyles.boldLabel);

        for (int i = 0; i < sectionLabels.Length; i++)
        {
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], sectionLabels[i] + " Moku", true, EditorStyles.foldoutHeader);

            if (foldouts[i])
            {
                EditorGUILayout.BeginVertical("box");

                // Animator
                SerializedProperty animatorProp = serializedObject.FindProperty(sectionLabels[i] + "Animator");
                if (animatorProp != null)
                {
                    EditorGUILayout.PropertyField(animatorProp, new GUIContent("Ruler Panel Animator"));
                }

                // MeshRenderer
                SerializedProperty renderers = serializedObject.FindProperty("lokahiWheelRenderers");
                if (renderers != null && i < renderers.arraySize)
                {
                    SerializedProperty renderer = renderers.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(renderer, new GUIContent("Wheel Renderer"));
                }

                // MaterialSet
                SerializedProperty materials = serializedObject.FindProperty("lokahiWheelMaterials");
                if (materials != null && i < materials.arraySize)
                {
                    SerializedProperty materialSet = materials.GetArrayElementAtIndex(i);
                    SerializedProperty materialArray = materialSet.FindPropertyRelative("materials");

                    EditorGUILayout.LabelField("Wheel Materials", EditorStyles.miniBoldLabel);
                    EditorGUI.indentLevel++;
                    for (int j = 0; j < materialArray.arraySize && j < materialNames.Length; j++)
                    {
                        EditorGUILayout.PropertyField(materialArray.GetArrayElementAtIndex(j), new GUIContent(materialNames[j]));
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
