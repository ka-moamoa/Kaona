using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioTextSynchronizer.Core
{
    [Serializable]
    [CreateAssetMenu(fileName = "Timings", menuName = "Audio Text Synchronizer/Audio Timings", order = 51)]
    public class PhraseAsset : ScriptableObject
    {
        public AudioClip Clip;
        [Multiline] public string Text;
        public List<Timing> Timings = new List<Timing>();
    }
}