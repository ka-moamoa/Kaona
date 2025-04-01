using System;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.TextHighlighters.Base;
using AudioTextSynchronizer.TextSplitters.Base;
using UnityEngine;

namespace AudioTextSynchronizer.TextEffects.Base
{
	public abstract class TextEffectBase : ScriptableObject, IDisposable
	{
		public event Action<Timing> OnTimingEnter;
		public event Action<Timing> OnTimingStart;
		public event Action<Timing, float> OnTimingProgress;
		public event Action<Timing> OnTimingEnd;
		
		[Header("General Parameters")]
		public TextSplitConfigBase TextSplitConfig;
		public TextHighlightConfigBase TextHighlightConfig;
		
		[NonSerialized] private int currentTextPartsIndex;
		public int CurrentTextPartsIndex
		{
			get => currentTextPartsIndex;
			protected set => currentTextPartsIndex = value < TextSplitConfig.TextParts.Count ? value : 0;
		}

		public TextPart CurrentPart => TextSplitConfig.TextParts[currentTextPartsIndex];

		[NonSerialized] protected TextSynchronizer TextSynchronizer;

		public virtual void Init(TextSynchronizer textSynchronizer)
		{
			currentTextPartsIndex = 0;
			TextSynchronizer = textSynchronizer;
			TextSplitConfig.Init(TextSynchronizer.Timings);
			OnTimingEntered(textSynchronizer.Timings.Timings[0]);
			SetTextToComponent(TextSplitConfig.TextParts[0].Text);
		}
				
		public virtual void Dispose()
		{
		}

		public virtual void OnTimingEntered(Timing timing)
		{
			OnTimingEnter?.Invoke(timing);
			TextHighlightConfig.SetTextPart(CurrentPart.Text);
		}

		public virtual void OnTimingStarted(Timing timing)
		{
			OnTimingStart?.Invoke(timing);
		}

		public virtual void OnTimingMoving(Timing timing, float progress)
		{
			OnTimingProgress?.Invoke(timing, progress);
		}

		public virtual void OnTimingFinished(Timing timing)
		{
			OnTimingEnd?.Invoke(timing);
		}

		public virtual void OnEffectFinished()
		{
		}

		public virtual void SkipPart()
		{
		}
		
		protected virtual void SetTextToComponent(string text)
		{
			var component = TextSynchronizer.TextComponent;
			var property = TextSynchronizer.TextComponent.GetType().GetProperty(TextSynchronizer.Property);
			if (property != null)
			{
				property.SetValue(component, text, null);
			}
		}
	}
}