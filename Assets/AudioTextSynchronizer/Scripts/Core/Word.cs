namespace AudioTextSynchronizer.Core
{
    public class Word
    {
        public readonly int StartIndex;
        public readonly int Length;

        public Word(int start, int length)
        {
            StartIndex = start;
            Length = length;
        }
    }
}