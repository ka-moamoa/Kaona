using System.Collections.Generic;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.TextEffects.Base;
using UnityEngine;

namespace AudioTextSynchronizer.TextSplitters.Base
{
    public abstract class TextSplitConfigBase : ScriptableObject
    {
        public IReadOnlyList<TextPart> TextParts => Parts;
        
        protected readonly List<TextPart> Parts = new List<TextPart>();
        protected PhraseAsset Phrases;

        public virtual void Init(PhraseAsset phraseAsset)
        {
            Phrases = phraseAsset;
            Parts.Clear();
            DivideTextForParts();
        }
        
        public virtual int GetNextTimingIndex(int currentTextPartsIndex, int currentTimingIndex)
        {
            return Mathf.Clamp(currentTimingIndex + 1, 0, Phrases.Timings.Count - 1);
        }

        public bool HavePartOfIndex(int charIndex)
        {
            foreach (var part in TextParts)
            {
                if (charIndex >= part.StartIndex && charIndex < part.EndIndex)
                {
                    return true;
                }
            }
            return false;
        }

        protected abstract void DivideTextForParts();
    }
}