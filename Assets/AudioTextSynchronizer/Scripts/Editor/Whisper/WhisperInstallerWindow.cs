using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace AudioTextSynchronizer.Editor.Whisper
{
    public class WhisperInstallerWindow : EditorWindow
    {
        private WhisperPackageDownloader whisperPackageDownloader;
        private WhisperModelsDownloader whisperModelsDownloader;
        private GUIStyle labelStyle;

        [MenuItem("Window/Audio Text Synchronizer/Whisper Installer", false, 112)]
        private static void OpenWindow()
        {
            GetWindow<WhisperInstallerWindow>("Whisper Installer", true);
        }

        private void OnEnable()
        {
            var instance = GetWindow<WhisperInstallerWindow>("Whisper Installer", true);
            instance.minSize = new Vector2(480, 235);
            instance.maxSize = new Vector2(480, 235);
            
            if (whisperPackageDownloader == null)
            {
                whisperPackageDownloader = new WhisperPackageDownloader();
            }
            
            if (whisperModelsDownloader == null)
            {
                whisperModelsDownloader = new WhisperModelsDownloader();
            }
        }

        private void Update()
        {
            if (whisperModelsDownloader.DownloadingStarted)
            {
                Repaint();
            }
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        private void OnGUI()
        {
            GUILayout.Label("    To generate timings automatically Audio-Text Synchronizer uses whisper.unity " +
                            "package which provide high-performance inference of OpenAI's Whisper automatic speech " +
                            "recognition (ASR) model running on your local machine.", EditorStyles.wordWrappedLabel);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("More information:");

#if UNITY_2021_1_OR_NEWER
            if (EditorGUILayout.LinkButton("whisper.unity"))
            {
                Application.OpenURL("https://github.com/Macoron/whisper.unity");
            }
            if (EditorGUILayout.LinkButton("OpenAI's Whisper"))
            {
                Application.OpenURL("https://github.com/openai/whisper");
            }
#else
            EditorGUILayoutExtension.UrlLabelField("whisper.unity", "https://github.com/Macoron/whisper.unity");
            EditorGUILayoutExtension.UrlLabelField("OpenAI's Whisper", "https://github.com/openai/whisper");
#endif
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawHorizontalLine();
            
            GUILayout.Label("    Please install whisper.unity and at least one model to recognize text from " +
                            "audio clip automatically. The smallest model means worse quality but works faster " +
                            "compared to other models. The bigger model size means better quality but works slower and uses more memory.", EditorStyles.wordWrappedLabel);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("1. whisper.unity");
#if WHISPER_UNITY
            var whisperInstalled = true;
#else
            var whisperInstalled = false;
#endif
            EditorGUI.BeginDisabledGroup(whisperInstalled);
            if (GUILayout.Button(whisperInstalled ? "Installed" : "Install"))
            {
                if (whisperPackageDownloader.DownloadRequest == null)
                {
                    whisperPackageDownloader.InstallWhisperUnity();
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("2. Whisper model");
            whisperModelsDownloader.DrawGUI();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            whisperModelsDownloader.DrawProgressBar();
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                };
            }
#if !WHISPER_UNITY
            EditorGUILayout.LabelField("whisper.unity is not installed!", labelStyle);
#else
            if (whisperModelsDownloader != null && !whisperModelsDownloader.DownloadingStarted)
            {
                EditorGUILayout.LabelField(whisperModelsDownloader.HaveAnyModel
                    ? "You're all set!"
                    : "Can't find any models in Assets/StreamingAssets/Whisper/ folder.", labelStyle);
            }
#endif
            GUILayout.EndHorizontal();
        }

        private static void DrawHorizontalLine()
        {
            GUILayout.Space(5f);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2f), Color.gray);
            GUILayout.Space(5f);
        }
    }
}