using System;
using System.IO;
using System.Threading.Tasks;
using AudioTextSynchronizer.Whisper;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace AudioTextSynchronizer.Editor.Whisper
{
    public class WhisperModelsDownloader
    {
        public bool IsTryingToGetModelSizes => isTryingToGetModelSizes;
        public bool DownloadingStarted => downloadingStarted;
        public bool HaveAnyModel => haveAnyModel || CheckIfHaveAnyModel();
         
        private int selectedModelIndex;
        private bool isTryingToGetModelSizes;
        private UnityWebRequest downloadRequest;
        private bool downloadingStarted;
        private bool cancelDownload;
        private bool haveAnyModel;
        private double lastCheckModelExistTime;
        private string[] modelNames = { "tiny.en", "tiny", "base.en", "base", "small.en", "small", "medium.en", "medium", "large-v1", "large" };
        private string[] modelNamesWithSize = { "tiny.en", "tiny", "base.en", "base", "small.en", "small", "medium.en", "medium", "large-v1", "large" };
        private const string ModelURLFormat = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-{0}.bin";

        public async void DrawGUI(params GUILayoutOption[] popupOptions)
        {
            EditorGUILayout.BeginHorizontal();
            selectedModelIndex = EditorGUILayout.Popup(selectedModelIndex, modelNamesWithSize, popupOptions);
            var downloadModel = false;
            if (!DownloadingStarted)
            {
                downloadModel = GUILayout.Button("Download", GUILayout.MaxWidth(80));
            }
            else if (downloadRequest != null)
            {
                if (GUILayout.Button("Cancel", GUILayout.MaxWidth(80)))
                {
                    downloadRequest.Abort();
                }
            }

            EditorGUI.BeginDisabledGroup(!DownloadingStarted);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            if (downloadModel)
            {
                var url = string.Format(ModelURLFormat, modelNames[selectedModelIndex]);
                await DownloadModel(url);
            }
            
            await CheckModelSize();
        }

        public void DrawProgressBar()
        {
            if (downloadRequest != null && !downloadRequest.isDone)
            {
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), downloadRequest.downloadProgress, $"{downloadRequest.downloadProgress * 100f:0.##}%");
            }
        }

        private bool CheckIfHaveAnyModel()
        {
            if (EditorApplication.timeSinceStartup - lastCheckModelExistTime < 1f)
                return false;
            
            var whisperPath = Path.Combine(Application.streamingAssetsPath, "Whisper");
            if (Directory.Exists(whisperPath))
            {
                var modelFiles = Directory.GetFiles(whisperPath, "*.bin");
                if (modelFiles.Length > 0)
                {
                    haveAnyModel = true;
                    return true;
                }
            }

            lastCheckModelExistTime = EditorApplication.timeSinceStartup;
            haveAnyModel = false;
            return false;
        }

        private async Task DownloadModel(string url)
        {
            if (downloadingStarted)
                return;
            
            downloadingStarted = true;
            downloadRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            var fileName = Path.GetFileName(url);
            var projectPath = Path.Combine(Application.dataPath, "../");
            var tempPath = Path.Combine(Path.GetFullPath(projectPath), FileUtil.GetUniqueTempPathInProject());
            var localPath = Path.Combine(Application.streamingAssetsPath + $"/Whisper/{fileName}");
            downloadRequest.downloadHandler = new DownloadHandlerFile(tempPath);
            downloadRequest.SendWebRequest();
            
            while (!downloadRequest.isDone)
            {
                await Task.Yield();
            }
            
#if UNITY_2020_1_OR_NEWER
            if (downloadRequest.result != UnityWebRequest.Result.Success)
#else
            if (!string.IsNullOrWhiteSpace(downloadRequest.error))
#endif
            {
                Debug.LogError(downloadRequest.error);
            }
            else
            {
                if (!Directory.Exists(Application.streamingAssetsPath))
                {
                    Directory.CreateDirectory(Application.streamingAssetsPath);
                }
                
                var whisperPath = Path.Combine(Application.streamingAssetsPath, "Whisper");
                if (!Directory.Exists(whisperPath))
                {
                    Directory.CreateDirectory(whisperPath);
                }
                
                FileUtil.ReplaceFile(tempPath, localPath);
                var relativePath = "Assets" + localPath.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(relativePath);
                AssetDatabase.Refresh();

                var whisperSettings = Resources.Load<WhisperSettings>("WhisperSettings");
                if (whisperSettings != null)
                {
                    whisperSettings.ModelPath = $"Whisper/{fileName}";
                    EditorUtility.SetDirty(whisperSettings);
#if UNITY_2021_3_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(whisperSettings);
#endif   
                }
            }

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            downloadRequest.Dispose();
            downloadRequest = null;
            downloadingStarted = false;
        }

        private async Task CheckModelSize()
        {
            if (isTryingToGetModelSizes)
                return;
            
            isTryingToGetModelSizes = true;
            for (var i = 0; i < modelNames.Length; i++)
            {
                var url = string.Format(ModelURLFormat, modelNames[i]);
                var webRequest = UnityWebRequest.Head(url);
                webRequest.SendWebRequest();
                while (!webRequest.isDone)
                {
                    await Task.Yield();
                }
                var size = Convert.ToInt64(webRequest.GetResponseHeader("Content-Length")) / 1024 / 1024;
                if (size > 0)
                {
                    modelNamesWithSize[i] = modelNames[i] + $" - {size} mb" ;
                }
            }
            isTryingToGetModelSizes = false;
        }
    }
}