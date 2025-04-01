using System;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace AudioTextSynchronizer.Editor.Whisper
{
    public class WhisperPackageDownloader
    {
        public AddRequest DownloadRequest => downloadRequest;
        private AddRequest downloadRequest;

        private const string PackageURL = "https://github.com/Macoron/whisper.unity.git?path=/Packages/com.whisper.unity#1.2.0";
        
        public async void InstallWhisperUnity(Action onStart = null, Action onFinish = null)
        {
            downloadRequest = Client.Add(PackageURL);
            Debug.Log("Package installation started...");
            onStart?.Invoke();
            while (!downloadRequest.IsCompleted)
            {
                await Task.Yield();
            }
            onFinish?.Invoke();
            Debug.Log("Package installation finished!");
            if (downloadRequest.Status == StatusCode.Success)
            {
                Debug.Log($"Package successfully installed!");
            }
            else if (downloadRequest.Status == StatusCode.Failure)
            {
                Debug.LogError($"Error while installing package: {downloadRequest.Error.message}");
            }
            downloadRequest = null;
        }
    }
}