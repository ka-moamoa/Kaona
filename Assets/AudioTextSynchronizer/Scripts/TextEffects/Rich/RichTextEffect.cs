using System;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.TextSplitters;
using AudioTextSynchronizer.Tools;
using UnityEngine;

namespace AudioTextSynchronizer.TextEffects
{
	[CreateAssetMenu(fileName = nameof(RichTextEffect), menuName = "Audio Text Synchronizer/Text Effects/Rich/" + nameof(RichTextEffect))]
	public class RichTextEffect : TextEffectBase
	{
		public string StartTag = "<color=#{0}>";
		public string EndTag = "</color>";
		public Color32 HighlightColor;
		
		[Header("Rich Text Parameters")] 
		public int CharIndexOffset;
		public OnTextAction EffectFinishedAction;

		[NonSerialized] private string tagStart;
		private string TagStart => tagStart;
		
		public override void Init(TextSynchronizer textSynchronizer)
		{
			base.Init(textSynchronizer);
			tagStart = string.Format(StartTag, HighlightColor.ColorToHex());
			OnTimingEntered(textSynchronizer.Timings.Timings[0]);
			SetTextToComponent(TagStart + EndTag + TextSplitConfig.TextParts[0].Text);
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
			
			if (!CurrentPart.IsPartOf(charIndex))
			{
				base.OnTimingFinished(timing);
				if (TextSplitConfig.HavePartOfIndex(charIndex))
				{
					CurrentTextPartsIndex++;
					base.OnTimingEntered(timing);
					SetTextToComponent(CurrentPart.Text);
					return;
				}
			}

			var highlightPositions = TextHighlightConfig.GetHighlightPositions(CurrentPart, currentPosition, timing.Text, indexOfText);
			var startTagPosition = highlightPositions[0];
			var endTagPosition = Mathf.Clamp(highlightPositions[1], 0, CurrentPart.Text.Length);
			var text = CurrentPart.Text.Insert(startTagPosition, TagStart).Insert(endTagPosition + TagStart.Length, EndTag);
			SetTextToComponent(text);
		}

		public override void OnEffectFinished()
		{
			base.OnEffectFinished();
			if (EffectFinishedAction == OnTextAction.ClearText)
			{
				SetTextToComponent(string.Empty);
			}
			else if (EffectFinishedAction == OnTextAction.SetCurrentPartText)
			{
				SetTextToComponent(CurrentPart.Text);
			}
		}
	}
}