using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.Tools.Tags;
using UnityEngine;

namespace AudioTextSynchronizer.TextHighlighters.Base
{
    public abstract class TextHighlightConfigBase : ScriptableObject
    {
        public bool ProcessInnerTags = true;

        private TagsParser tagsParser;

        public void SetTextPart(string text)
        {
            if (ProcessInnerTags)
            {
                tagsParser = new TagsParser(text);
            }
        }

        public virtual int[] GetHighlightPositions(TextPart currentPart, int currentPosition, string currentTiming, int indexOfText)
        {
            var endCodePosition = GetCodePosition(currentPosition);
            return new[] { 0, endCodePosition };
        }

        protected int GetCodePosition(int codePosition)
        {
            if (ProcessInnerTags)
            {
                foreach (var tag in tagsParser.Tags)
                {
                    if (tag.IsInsideTag(codePosition))
                    {
                        if (tag.IsInsideTagCodes(codePosition))
                        {
                            codePosition = tag.GetSafePosition(codePosition);
                        }
                    }
                }
            }
            return codePosition;
        }
    }
}