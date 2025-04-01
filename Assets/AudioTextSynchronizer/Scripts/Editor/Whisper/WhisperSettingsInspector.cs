using System.IO;
using System.Threading.Tasks;
using AudioTextSynchronizer.Whisper;
using UnityEditor;
using UnityEngine;

namespace AudioTextSynchronizer.Editor.Whisper
{
    [CustomEditor(typeof(WhisperSettings))]
    public class WhisperSettingsInspector : UnityEditor.Editor
    {
        private SerializedProperty modelPathProperty;
        private int selectedModelPopupIndex;
        private WhisperModelsDownloader whisperModelsDownloader;
        
        void OnEnable()
        {
            modelPathProperty = serializedObject.FindProperty("ModelPath");
            if (whisperModelsDownloader == null)
            {
                whisperModelsDownloader = new WhisperModelsDownloader();
            }
        }

        public override bool RequiresConstantRepaint()
        {
            if (whisperModelsDownloader.DownloadingStarted || whisperModelsDownloader.IsTryingToGetModelSizes)
                return true;
            
            return base.RequiresConstantRepaint();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            var beginHorizontal = true;
            EditorGUILayout.PropertyField(modelPathProperty, new GUIContent(string.Empty));
            var markAsDirty = false;
            if (GUILayout.Button(new GUIContent("Select", "Load existing model"), GUILayout.MaxWidth(80)))
            {
                var whisperPath = Path.Combine(Application.streamingAssetsPath, "Whisper");
                EditorGUILayout.EndHorizontal();
                beginHorizontal = false;
                var path = EditorUtility.OpenFilePanelWithFilters("Title", whisperPath, new []{"Model file", "bin"});
                if (!string.IsNullOrWhiteSpace(path))
                {
                    path = path.Substring(Application.streamingAssetsPath.Length).TrimStart(Path.AltDirectorySeparatorChar);
                    modelPathProperty.stringValue = path;
                    serializedObject.ApplyModifiedProperties();
                    markAsDirty = true;
                }
            }
            
            if (beginHorizontal)
            {
                EditorGUILayout.EndHorizontal();
            }

            if (markAsDirty)
            {
                EditorUtility.SetDirty(target);
#if UNITY_2021_3_OR_NEWER
                AssetDatabase.SaveAssetIfDirty(target);
#endif   
            }

            whisperModelsDownloader.DrawGUI();
            whisperModelsDownloader.DrawProgressBar();
            
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
#if UNITY_2021_1_OR_NEWER
            if (EditorGUILayout.LinkButton("You can manually download models weights here"))
            {
                Application.OpenURL("https://huggingface.co/ggerganov/whisper.cpp");
            }
#else
            EditorGUILayoutExtension.UrlLabelField("You can download models manually here", "https://huggingface.co/ggerganov/whisper.cpp");
#endif              
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}