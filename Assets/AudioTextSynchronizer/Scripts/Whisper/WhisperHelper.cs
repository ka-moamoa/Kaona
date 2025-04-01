#if WHISPER_UNITY
using System.Linq;
using System.Threading.Tasks;
using AudioTextSynchronizer.Core;
using UnityEngine;
using Whisper;

namespace AudioTextSynchronizer.Whisper
{
    public class WhisperHelper
    {
        private WhisperWrapper whisper;
        private WhisperParams defaultParameters;
        private const float ColorRangeFrom = 0.15f;
        private const float ColorRangeTo = 0.85f;

        public WhisperHelper(string language = "auto")
        {
            defaultParameters = WhisperParams.GetDefaultParams();
            defaultParameters.Language = language;
            defaultParameters.Translate = false;
            defaultParameters.NoContext = true;
            defaultParameters.SingleSegment = false;
            defaultParameters.SpeedUp = false;
            defaultParameters.AudioCtx = 0;
            defaultParameters.EnableTokens = false;
            defaultParameters.TokenTimestamps = false;
            defaultParameters.InitialPrompt = string.Empty;
        }

        public async Task<WhisperWrapper> LoadModel()
        {
            var modelPath = Resources.Load<WhisperSettings>("WhisperSettings").FullModelPath;
            whisper = await WhisperWrapper.InitFromFileAsync(modelPath);
            return whisper;
        }
        
        public async Task<WhisperWrapper> LoadModel(string modelPath)
        {
            whisper = await WhisperWrapper.InitFromFileAsync(modelPath);
            return whisper;
        }
        
        public async Task<PhraseAsset> GenerateTimings(AudioClip clip, string language = "auto")
        {
            defaultParameters.Language = language;
            var result = await whisper.GetTextAsync(clip, defaultParameters);
            var asset = CreatePhraseAsset(clip, result);
            return asset;
        }

        public async Task<PhraseAsset> GenerateTimings(AudioClip clip, WhisperParams parameters)
        {
            var result = await whisper.GetTextAsync(clip, parameters);
            var asset = CreatePhraseAsset(clip, result);
            return asset;
        }
        
        public async Task<PhraseAsset> GenerateTimings(float[] data, int frequency, int channels)
        {
            var result = await whisper.GetTextAsync(data, frequency, channels, defaultParameters);
            var clip = AudioClip.Create("SomeName", data.Length, channels, frequency, false);
            clip.SetData(data, 1);
            var asset = CreatePhraseAsset(clip, result);
            return asset;
        }

        private PhraseAsset CreatePhraseAsset(AudioClip clip, WhisperResult whisperResult)
        {
            var phraseAsset = ScriptableObject.CreateInstance<PhraseAsset>();
            phraseAsset.Clip = clip;
            var text = new string[whisperResult.Segments.Count];
            for (var i = 0; i < whisperResult.Segments.Count; i++)
            {
                var subtitle = whisperResult.Segments[i];
                if (subtitle.Text == "[BLANK_AUDIO]")
                    continue;

                var startTime = (float)subtitle.Start.TotalMilliseconds / 1000f;
                startTime = Mathf.Max(0, startTime);
                var endTime = (float)subtitle.End.TotalMilliseconds / 1000f;
                endTime = Mathf.Min(endTime, clip.length);
                var timingText = subtitle.Text.Trim();
                var color = new Color(Random.Range(ColorRangeFrom, ColorRangeTo), Random.Range(ColorRangeFrom, ColorRangeTo), Random.Range(ColorRangeFrom, ColorRangeTo), 41f / 255f);
                var name = string.Join(" ", timingText.Split(' ').Take(3));
                var newTiming = new Timing(startTime, endTime, color, name, timingText);
                phraseAsset.Timings.Add(newTiming);
                text[i] = timingText;
            }
            phraseAsset.Text = string.Join(" ", text);
            return phraseAsset;
        }
    }
}
#endif