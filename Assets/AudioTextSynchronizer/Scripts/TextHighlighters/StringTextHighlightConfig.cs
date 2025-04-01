using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.TextHighlighters.Base;
using UnityEngine;

namespace AudioTextSynchronizer.TextHighlighters
{
    [CreateAssetMenu(fileName = nameof(StringTextHighlightConfig), menuName = "Audio Text Synchronizer/Text Highlighters/" + nameof(StringTextHighlightConfig))]
    public class StringTextHighlightConfig : TextHighlightConfigBase
    {
        public char[] IndentsChar = {' ', '\t', '\n'};

        public override int[] GetHighlightPositions(TextPart currentPart, int currentPosition, string currentTiming, int indexOfText)
        {
            var lastIndex = Mathf.Clamp(currentPosition, 0, currentPart.Text.Length - 1);
            var startCodePosition = currentPart.Text.LastIndexOfAny(IndentsChar, lastIndex);
            startCodePosition = GetCodePosition(Mathf.Clamp(startCodePosition, 0, int.MaxValue));
            var endCodePosition = currentPart.Text.IndexOfAny(IndentsChar, startCodePosition + 1);
            if (endCodePosition == -1)
            {
                endCodePosition = currentPart.Text.Length;
            }
            endCodePosition = GetCodePosition(endCodePosition);
            return new[] { startCodePosition, endCodePosition };
        }
    }
}