using System;
using System.IO;
using UnityEngine;

namespace AudioTextSynchronizer.Whisper
{
    [Serializable]
    // [CreateAssetMenu(fileName = "WhisperSettings", menuName = "Audio Text Synchronizer/Whisper Settings", order = 52)]
    public class WhisperSettings : ScriptableObject
    {
        public string ModelPath = "Whisper/ggml-tiny.bin";

        public string FullModelPath => Path.Combine(Application.streamingAssetsPath, ModelPath);
    }
}