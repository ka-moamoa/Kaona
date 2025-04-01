using UnityEditor;
using UnityEngine;
#if WHISPER_UNITY
using System.IO;
using AudioTextSynchronizer.Whisper;
#else
using AudioTextSynchronizer.Editor.Whisper;
#endif

namespace AudioTextSynchronizer.Editor
{
    public class AudioClipsMenu
    {
        [MenuItem("Assets/Create/Audio Text Synchronizer/Generate Timings", false, 20)]
#if WHISPER_UNITY
        private static async void GenerateTimings()
#else
        private static void GenerateTimings()
#endif
        {
#if WHISPER_UNITY
            var helper = new WhisperHelper();
            await helper.LoadModel();
            var selectedObjects = Selection.objects;
            foreach (var selectedObject in selectedObjects)
            {
                if (selectedObject is AudioClip audioClip)
                {
                    var phraseAsset = await helper.GenerateTimings(audioClip);
                    var audioClipPath = AssetDatabase.GetAssetPath(audioClip);
                    var assetPath = Path.Combine(Path.GetDirectoryName(audioClipPath) ?? string.Empty, Path.GetFileNameWithoutExtension(audioClipPath) + ".asset");
                    phraseAsset.hideFlags = HideFlags.None;
                    AssetDatabase.CreateAsset(phraseAsset, assetPath);
                    EditorUtility.SetDirty(phraseAsset);
#if UNITY_2021_3_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(phraseAsset);
#endif
                }
            }
            
            AssetDatabase.Refresh();
#else
            var whisperInstallWindow = (WhisperInstallerWindow) EditorWindow.GetWindow(typeof(WhisperInstallerWindow), false, "Whisper Installer");
            whisperInstallWindow.Show(true);
#endif
        }
        
        [MenuItem("Assets/Create/Audio Text Synchronizer/Generate Timings", true)]
        static bool ValidateAudioClips()
        {
            var selectedObjects = Selection.objects;
            if (selectedObjects.Length == 0)
                return false;
            
            foreach (var selectedObject in selectedObjects)
            {
                if (!(selectedObject is AudioClip))
                {
                    return false;
                }
            }
            return true;
        }
    }
}