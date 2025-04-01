using System;
using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.TextSplitters.Base;
using UnityEngine;

namespace AudioTextSynchronizer.TextSplitters
{
    [CreateAssetMenu(fileName = nameof(TimingsTextSplitConfig), menuName = "Audio Text Synchronizer/Text Splitters/" + nameof(TimingsTextSplitConfig))]
    public class TimingsTextSplitConfig : TextSplitConfigBase
    {
        protected override void DivideTextForParts()
        {
            var startFrom = 0;
            for (var i = 0; i < Phrases.Timings.Count; i++)
            {
                var timing = Phrases.Timings[i];
                var startIndex = Phrases.Text.IndexOf(timing.Text, startFrom, StringComparison.Ordinal);
                startFrom = startIndex + timing.Text.Length;
                Parts.Add(new TextPart
                {
                    StartIndex = startIndex,
                    EndIndex = startIndex + timing.Text.Length,
                    StartsFromTimingIndex = i,
                    Text = timing.Text
                });
            }
        }
    }
}
