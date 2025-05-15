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

        // Top-level animator
        SerializedProperty rulerPanelAnim = serializedObject.FindProperty("RulerPanelAnim");
        EditorGUILayout.PropertyField(rulerPanelAnim, new GUIContent("Ruler Panel Animator"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Moku Properties", EditorStyles.boldLabel);

        // Get all arrays once
        SerializedProperty renderers = serializedObject.FindProperty("lokahiWheelRenderers");
        SerializedProperty materials = serializedObject.FindProperty("lokahiWheelMaterials");
        SerializedProperty mokus = serializedObject.FindProperty("mokus");
        SerializedProperty audioArray = serializedObject.FindProperty("MokuStartAudio");

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

                // Renderer
                if (renderers != null && i < renderers.arraySize)
                {
                    SerializedProperty renderer = renderers.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(renderer, new GUIContent("Wheel Renderer"));
                }

                // MaterialSet
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

                // Moku state GameObjects
                if (mokus != null && i < mokus.arraySize)
                {
                    SerializedProperty moku = mokus.GetArrayElementAtIndex(i);
                    SerializedProperty introProp = moku.FindPropertyRelative("introTileDone");
                    SerializedProperty unhealedProp = moku.FindPropertyRelative("unhealed");
                    SerializedProperty healedProp = moku.FindPropertyRelative("healed");

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Moku State Objects", EditorStyles.miniBoldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(introProp, new GUIContent("Intro Tile"));
                    EditorGUILayout.PropertyField(unhealedProp, new GUIContent("Unhealed Tile"));
                    EditorGUILayout.PropertyField(healedProp, new GUIContent("Healed Tile"));
                    EditorGUI.indentLevel--;
                }

                // AudioSource
                if (audioArray != null && i < audioArray.arraySize)
                {
                    SerializedProperty audioSource = audioArray.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(audioSource, new GUIContent("Start Audio"));
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
