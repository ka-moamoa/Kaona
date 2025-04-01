using System;

namespace AudioTextSynchronizer.TextEffects.Base
{
    [Serializable]
    public class TextPart
    {
        public int StartIndex;
        public int EndIndex;
        public int StartsFromTimingIndex;
        public string Text;

        public bool IsPartOf(int index)
        {
            return index >= StartIndex && index <= EndIndex;
        }
    }
}