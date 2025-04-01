using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.TextSplitters.Base;
using UnityEngine;

namespace AudioTextSynchronizer.TextSplitters
{
    [CreateAssetMenu(fileName = nameof(DefaultTextSplitConfig), menuName = "Audio Text Synchronizer/Text Splitters/" + nameof(DefaultTextSplitConfig))]
    public class DefaultTextSplitConfig : TextSplitConfigBase
    {
        protected override void DivideTextForParts()
        {
            Parts.Add(new TextPart
            {
               StartIndex = 0,
               EndIndex = Phrases.Text.Length,
               StartsFromTimingIndex = 0,
               Text = Phrases.Text
            });
        }
    }
}
