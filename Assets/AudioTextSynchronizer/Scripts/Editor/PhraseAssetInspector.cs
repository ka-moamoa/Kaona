using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.Editor.Timings;
using UnityEditor;
using UnityEngine;

namespace AudioTextSynchronizer.Editor
{
    [CustomEditor(typeof(PhraseAsset))]
    public class PhraseAssetInspector : UnityEditor.Editor
    {
        private SerializedProperty clip;
        private SerializedProperty text;
        private SerializedProperty timings;

        void OnEnable()
        {
            clip = serializedObject.FindProperty("Clip");
            text = serializedObject.FindProperty("Text");
            timings = serializedObject.FindProperty("Timings");
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var assetPath = AssetDatabase.GetAssetPath(instanceId);
            var phraseAsset = AssetDatabase.LoadAssetAtPath<PhraseAsset>(assetPath);
            if (phraseAsset != null)
            {
                OpenEditorWindow(phraseAsset);
                return true;
            }
            return false;
        }

        private static void OpenEditorWindow(PhraseAsset phraseAsset)
        {            
            if (TimingsWindow.instance != null && EditorWindow.HasOpenInstances<TimingsWindow>())
            {
                if (TimingsWindow.instance.Data != null && !EditorUtility.DisplayDialog("Confirmation",
                        "Are you sure you want to open PhraseAsset? Current changes might be lost.", "Yes", "Cancel"))
                {
                    return;
                }

                var assetPath = AssetDatabase.GetAssetPath(phraseAsset);
                TimingsWindow.instance.Init(true, assetPath);
                TimingsWindow.instance.Repaint();
                EditorWindow.FocusWindowIfItsOpen<TimingsWindow>();
            }
            else
            {
                var timingsWindow = (TimingsWindow) EditorWindow.GetWindow(typeof(TimingsWindow), false, TimingsWindow.Title);
                var assetPath = AssetDatabase.GetAssetPath(phraseAsset);
                timingsWindow.Init(true, assetPath);
                timingsWindow.Show();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var phraseAsset = (PhraseAsset) target;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(clip, new GUIContent("Clip", "Audio Clip"));
            EditorGUILayout.PropertyField(text, new GUIContent("Text", "Text"));
            EditorGUILayout.PropertyField(timings, new GUIContent("Timings", "Timings list"));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(phraseAsset);
                AssetDatabase.SaveAssets();
                Repaint();
            }
            
            if (GUILayout.Button("Validate"))
            {
                var timingsValidator = new TimingsValidator();
                timingsValidator.ValidateTimings(phraseAsset);
            }
            
            if (GUILayout.Button("Open in Timings Editor"))
            {
                OpenEditorWindow(phraseAsset);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}