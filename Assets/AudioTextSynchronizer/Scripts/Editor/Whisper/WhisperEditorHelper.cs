using System.Collections.Generic;
using System.Threading.Tasks;
using AudioTextSynchronizer.Core;
using UnityEditor;
using UnityEngine;
#if WHISPER_UNITY
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AudioTextSynchronizer.Tools;
using AudioTextSynchronizer.Whisper;
using Whisper;
using Whisper.Native;
#endif

namespace AudioTextSynchronizer.Editor.Whisper
{
    public class WhisperEditorHelper
    {
#if WHISPER_UNITY
        private WhisperHelper whisperHelper = new WhisperHelper();
        private WhisperWrapper whisper;
#endif
        private WhisperPackageDownloader whisperPackageDownloader;
        private List<KeyValuePair<string, string>> allLanguages;
        private int languageIndex = int.MaxValue;
        private int LanguageIndex
        {
            get => languageIndex;
            set
            {
                languageIndex = value;
                EditorPrefs.SetInt("ATS_LanguageIndex", languageIndex);
            }
        }
        private bool isModelLoading;

#if WHISPER_UNITY
        private async Task<bool> LoadModel()
#else
        private bool LoadModel()
#endif
        {
#if WHISPER_UNITY
            if (whisper == null && !isModelLoading)
            {
                var whisperSettings = Resources.Load<WhisperSettings>("WhisperSettings");
                var modelPath = Path.Combine(Application.streamingAssetsPath, whisperSettings.ModelPath);
                isModelLoading = true;
                var path = Path.Combine(Application.dataPath, modelPath);
                if (!File.Exists(path))
                {
                    Debug.LogError($"Can't find model by path {path}");
                    isModelLoading = false;
                    return false;
                }
                EditorUtility.DisplayProgressBar("Model loading", "Loading model from file...", 0.25f);
                whisper = await whisperHelper.LoadModel(modelPath);
                EditorUtility.ClearProgressBar();
                isModelLoading = false;
            }
            return true;
#else
            return false;
#endif
        }

        public void DisplayLanguagesPopup()
        {
#if WHISPER_UNITY
            allLanguages = new List<KeyValuePair<string, string>>();
            var count = WhisperNative.whisper_lang_max_id() + 1;
            for (var i = 0; i < count; i++)
            {
                var strPtr = WhisperNative.whisper_lang_str(i);
                var lang = Marshal.PtrToStringAnsi(strPtr);
                if (lang != null && WhisperSupportedLanguages.Languages.TryGetValue(lang, out var language))
                {
                    allLanguages.Add(new KeyValuePair<string, string>(language.ToPascalCase(), lang));
                }
            }

            allLanguages = allLanguages.OrderBy(x => x.Key).ToList();
            allLanguages.Insert(0, new KeyValuePair<string, string>("Automatic", "auto"));

            var keys = allLanguages.Select(x => x.Key).ToArray();
            if (LanguageIndex >= allLanguages.Count)
            {
                LanguageIndex = Array.IndexOf(keys, "Automatic");
            }

            EditorGUILayout.LabelField("Language", GUILayout.MaxWidth(60));
            LanguageIndex = EditorGUILayout.Popup(languageIndex, keys, GUILayout.MaxWidth(110));
#endif
        }

        public async Task<PhraseAsset> GetData(AudioClip clip)
        {
#if WHISPER_UNITY
            if (whisper == null)
            {
                Debug.LogWarning("Model doesn't loaded! Trying to load the model...");
                if (!await LoadModel())
                {
                    var whisperInstallWindow = (WhisperInstallerWindow) EditorWindow.GetWindow(typeof(WhisperInstallerWindow), false, "Whisper Installer");
                    whisperInstallWindow.Show(true);
                    return null;
                }
            }

            PhraseAsset result = null;
            EditorUtility.DisplayProgressBar("Generating", "Generating timings...", 0.75f);
            try
            {
                result = await whisperHelper.GenerateTimings(clip, allLanguages[LanguageIndex].Value);
            }
            catch (Exception)
            {
                // ignored
            }
            EditorUtility.ClearProgressBar();

            if (result != null)
            {
                var data = result;
                return data;
            }
            return null;
#else
            await Task.Yield();
            var whisperInstallWindow = (WhisperInstallerWindow) EditorWindow.GetWindow(typeof(WhisperInstallerWindow), false, "Whisper Installer");
            whisperInstallWindow.Show(true);
            return null;
#endif
        }
    }
}