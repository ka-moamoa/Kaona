using System;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.TextEffects.Components;
using AudioTextSynchronizer.TextEffects.Components.Base;
using AudioTextSynchronizer.TextEffects.MeshAnimations.Base;
using AudioTextSynchronizer.TextSplitters;
using UnityEngine;
using UnityEngine.UI;
#if TEXTMESHPRO_3_0_OR_NEWER
using TMPro;
#endif

namespace AudioTextSynchronizer.TextEffects
{
    [CreateAssetMenu(fileName = nameof(MeshTextEffect), menuName = "Audio Text Synchronizer/Text Effects/Mesh/" + nameof(MeshTextEffect))]
    public class MeshTextEffect : TextEffectBase
    {
        [Header("Effect Parameters")]
        public MeshAnimationBase MeshAnimation;
        public int CharIndexOffset;
        public bool AnimateTextInstantly;
        public OnTextPartAction TextPartFinishedAction = OnTextPartAction.SetCurrentPartText;
     
        [NonSerialized] private TextMeshProcessorBase meshProcessor;
        private bool isMeshProcessorAdded;
        private string componentText;
        private readonly char[] indentsChar = { ' ', '\t', '\n' };

        public override void Init(TextSynchronizer textSynchronizer)
        {
            if (textSynchronizer.TextComponent.gameObject.TryGetComponent<TextMeshProcessorBase>(out var textMeshProcessor))
            {
                meshProcessor = textMeshProcessor;
                isMeshProcessorAdded = true;
            }
            else
            {
                if (textSynchronizer.TextComponent is Text)
                {
                    meshProcessor = textSynchronizer.TextComponent.gameObject.AddComponent<TextMeshProcessorUIText>();
                    isMeshProcessorAdded = true;
                }
#if TEXTMESHPRO_3_0_OR_NEWER
                else if (textSynchronizer.TextComponent is TMP_Text)
                {
                    meshProcessor = textSynchronizer.TextComponent.gameObject.AddComponent<TextMeshProcessorTMP>();
                    isMeshProcessorAdded = false;
                }
#endif
            }

            meshProcessor.MeshAnimation = MeshAnimation;
            meshProcessor.PreInit();
            base.Init(textSynchronizer);
            if (meshProcessor == null)
            {
                Debug.LogWarning("Can't find MeshProcessor!");
                return;
            }

            meshProcessor.FillInstantly = AnimateTextInstantly;
            meshProcessor.Init();
            meshProcessor.ResetProgress();
        }

        public override void OnTimingMoving(Timing timing, float progress)
        {
            base.OnTimingMoving(timing, progress);
            int startTimingIndex;
            if (TextSplitConfig is StringTextSplitConfig splitter)
            {
                startTimingIndex = TextSynchronizer.GetCharProgress(splitter.TrimCharacters);
            }
            else
            {
                startTimingIndex = TextSynchronizer.GetCharProgress();
            }
            var indexOfText = TextSynchronizer.Timings.Text.IndexOf(timing.Text, startTimingIndex, StringComparison.Ordinal);
            var currentIndex = (int)(progress * timing.Text.Length);
            currentIndex = Mathf.Clamp(currentIndex, 0, timing.Text.Length);
            var charIndex = indexOfText + currentIndex;
            var currentPosition = charIndex - CurrentPart.StartIndex + CharIndexOffset;

            if (TextPartFinishedAction == OnTextPartAction.ClearText && currentIndex >= 0 && componentText == string.Empty)
            {
                SetTextToComponent(CurrentPart.Text);
            }
            
            if (!CurrentPart.IsPartOf(charIndex))
            {
                base.OnTimingFinished(timing);
                if (TextSplitConfig.HavePartOfIndex(charIndex))
                {
                    CurrentTextPartsIndex++;
                    base.OnTimingEntered(timing);
                    meshProcessor.ResetProgress();
                    meshProcessor.OnPreSetNewText();
                    if (TextPartFinishedAction == OnTextPartAction.ClearText)
                    {
                        SetTextToComponent(string.Empty);
                    }
                    else if (TextPartFinishedAction == OnTextPartAction.SetCurrentPartText)
                    {
                        SetTextToComponent(CurrentPart.Text);
                    }
                    meshProcessor.OnSetNewText();
                    return;
                }
            }

            var highlightPositions = TextHighlightConfig.GetHighlightPositions(CurrentPart, currentPosition, timing.Text, indexOfText);
            var endCodePosition = Mathf.Clamp(highlightPositions[1], 0, CurrentPart.Text.Length);
            var skipLettersCount = 0;
            if (meshProcessor.SkipIndents)
            {
                foreach (var skipLetter in indentsChar)
                {
                    for (var j = 0; j < endCodePosition - CurrentPart.StartIndex; j++)
                    {
                        if (CurrentPart.Text.Length > j && CurrentPart.Text[j] == skipLetter)
                        {
                            skipLettersCount++;
                        }
                    }
                }
            }

            var index = Mathf.Clamp(endCodePosition - skipLettersCount, 0, CurrentPart.Text.Length - skipLettersCount);
            meshProcessor.OnCharProgress(index);
        }

        public override void OnEffectFinished()
        {
            if (TextPartFinishedAction == OnTextPartAction.SetCurrentPartText)
            {
                CurrentTextPartsIndex = TextSplitConfig.TextParts.Count - 1;
                SetTextToComponent(CurrentPart.Text);
            }
            else if (TextPartFinishedAction == OnTextPartAction.ClearText)
            {
                SetTextToComponent(string.Empty);
            }
            base.OnEffectFinished();
            meshProcessor.Finish();
            meshProcessor.ForceUpdate();
        }

        public override void SkipPart()
        {
            CurrentTextPartsIndex = (CurrentTextPartsIndex + 1) % TextSplitConfig.TextParts.Count;
            if (TextSplitConfig.TextParts.Count > 1)
            {
                meshProcessor.ResetProgress();
                meshProcessor.OnPreSetNewText();
                SetTextToComponent(CurrentPart.Text);
                meshProcessor.OnSetNewText();
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            componentText = null;
            if (meshProcessor != null)
            {
                meshProcessor.Dispose();
                if (isMeshProcessorAdded)
                {
                    DestroyImmediate(meshProcessor);
                }
            }
        }

        protected override void SetTextToComponent(string text)
        {
            componentText = text;
#if TEXTMESHPRO_3_0_OR_NEWER
            if (meshProcessor is TextMeshProcessorTMP textMeshProcessorTMP)
            {
                if (textMeshProcessorTMP.TextMeshPro.text == componentText)
                {
                    textMeshProcessorTMP.Graphic.SetVerticesDirty();
                    textMeshProcessorTMP.Graphic.SetLayoutDirty();
                }
            }
#endif
            base.SetTextToComponent(componentText);
        }
    }
}