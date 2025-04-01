using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.TextHighlighters.Base;
using UnityEngine;

namespace AudioTextSynchronizer.TextHighlighters
{
    [CreateAssetMenu(fileName = nameof(TimingTextHighlightConfig), menuName = "Audio Text Synchronizer/Text Highlighters/" + nameof(TimingTextHighlightConfig))]
    public class TimingTextHighlightConfig : TextHighlightConfigBase
    {
        public override int[] GetHighlightPositions(TextPart currentPart, int currentPosition, string currentTiming, int indexOfText)
        {
            var startCodePosition = GetCodePosition(indexOfText) - currentPart.StartIndex;
            var endCodePosition = GetCodePosition(indexOfText + currentTiming.Length) - currentPart.StartIndex;
            return new[] { startCodePosition, endCodePosition };
        }
    }
}