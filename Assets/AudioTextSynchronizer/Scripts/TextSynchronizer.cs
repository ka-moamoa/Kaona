using System;
using System.Collections.Generic;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.TextEffects.Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace AudioTextSynchronizer
{
	public class TextSynchronizer : MonoBehaviour
	{
		public event Action OnSyncFinished;
		public event Action<string, int> OnWordReached;
		
		[FormerlySerializedAs("GameObjectWithTextProperty")] [SerializeField] private GameObject gameObjectWithTextComponent;
		public GameObject GameObjectWithTextComponent
		{
			get => gameObjectWithTextComponent;
			set => gameObjectWithTextComponent = value;
		}
		
		[FormerlySerializedAs("Property")] [SerializeField] private string property;
		public string Property
		{
			get => property;
			set => property = value;
		}
		
		[FormerlySerializedAs("TextComponent")] [SerializeField] private Component textComponent;
		public Component TextComponent
		{
			get => textComponent;
			set => textComponent = value;
		}
		
		[FormerlySerializedAs("Source")] [SerializeField] private AudioSource source;
		public AudioSource Source
		{
			get => source;
			set => source = value;
		}
		
		[FormerlySerializedAs("Phrases")] [SerializeField] private PhraseAsset timings;
		public PhraseAsset Timings
		{
			get => timings;
			set
			{
				if (timings != value)
				{
					timings = value;
					Stop();
					SplitWords();
					InitEffect();
				}
			}
		}

		[FormerlySerializedAs("Effect")] [SerializeField] private TextEffectBase textEffect;
		public TextEffectBase TextEffect
		{
			get => textEffect;
			set
			{
				if (textEffect != null)
				{
					textEffect.Dispose();
				}
				isEffectInitialized = false;
				textEffect = value;
			}
		}

		[FormerlySerializedAs("IsRunning")] [SerializeField] private bool isRunning = true;
		public bool IsRunning
		{
			get => isRunning;
			set => isRunning = value;
		}

		private Timing CurrentTiming => timings.Timings[currentSubTimingIndex];
		private bool IsStarted => source.time >= CurrentTiming.StartPosition;
		private bool IsFinished => source.time >= CurrentTiming.EndPosition;

		private readonly List<Word> words = new List<Word>();
		private int wordIndexReached;
		private Timing previousTiming;
		private int currentSubTimingIndex;
		private bool isEffectInitialized;

		private void Start()
		{
			if (isActiveAndEnabled && source.playOnAwake)
			{
				Play();
			}
			SplitWords();
		}

		private void InitEffect()
		{
			textEffect.Init(this);
			textEffect.OnTimingEntered(CurrentTiming);
			isEffectInitialized = true;
		}

		public void Play(bool initializeEffect = false)
		{
			if (timings == null)
			{
				Debug.LogWarning("Timings is null! Please, set reference to the PhraseAsset (Timings).");
				return;
			}
			
			if (!isEffectInitialized || initializeEffect)
			{
				InitEffect();
			}
			isRunning = true;
			source.Play();
		}
		
		public void Pause()
		{
			isRunning = false;
			source.Pause();
		}

		public void Stop(bool resetCurrentTiming = true)
		{
			if (resetCurrentTiming)
			{
				previousTiming = null;
				currentSubTimingIndex = 0;
			}
			wordIndexReached = 0;
			isRunning = false;
			source.Stop();
			source.time = 0;
		}

		public void SkipPhrase()
		{
			var nextTimingIndex = textEffect.TextSplitConfig.GetNextTimingIndex(TextEffect.CurrentTextPartsIndex, currentSubTimingIndex);
			bool isLastPart;
			if (TextEffect.TextSplitConfig.TextParts.Count == 1)
			{
				isLastPart = nextTimingIndex == timings.Timings.Count - 1;
			}
			else
			{
				isLastPart = textEffect.TextSplitConfig.TextParts.Count == textEffect.CurrentTextPartsIndex + 1;
			}
			
			if (isLastPart)
			{
				currentSubTimingIndex = timings.Timings.Count - 1;
				source.time = CurrentTiming.EndPosition - float.Epsilon;
				Update();
			}
			else
			{
				var isNewSubTiming = nextTimingIndex > currentSubTimingIndex;
				if (isNewSubTiming)
				{
					textEffect.OnTimingFinished(CurrentTiming);
					currentSubTimingIndex = nextTimingIndex;
					source.time = CurrentTiming.StartPosition;
					textEffect.SkipPart();
					textEffect.OnTimingEntered(CurrentTiming);
				}
				else
				{
					var time = textEffect.CurrentPart.EndIndex / (float)CurrentTiming.Text.Length;
					time = Mathf.Lerp(CurrentTiming.StartPosition, CurrentTiming.EndPosition, time);
					source.time = time;
					textEffect.SkipPart();
				}
			}
		}

		public void SplitWords()
		{
			var splittedWords = timings.Text.Split(" \t\n:,.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var startFrom = 0;
			foreach (var word in splittedWords)
			{
				var indexOf = timings.Text.IndexOf(word, startFrom, StringComparison.Ordinal);
				var wordIndex = new Word(indexOf, word.Length);
				words.Add(wordIndex);
				startFrom = indexOf + word.Length;
			}
		}

		public int GetCharProgress()
		{
			var lastIndexOfPhrase = 0;
			for (var i = 0; i < currentSubTimingIndex; i++)
			{
				lastIndexOfPhrase = timings.Text.IndexOf(timings.Timings[i].Text, lastIndexOfPhrase, StringComparison.Ordinal) + timings.Timings[i].Text.Length;
			}
			return lastIndexOfPhrase;
		}
		
		public int GetCharProgress(char[] trim)
		{
			var lastIndexOfPhrase = 0;
			for (var i = 0; i < currentSubTimingIndex; i++)
			{
				var timingText = timings.Timings[i].Text.Trim(trim);
				lastIndexOfPhrase = timings.Text.IndexOf(timingText, lastIndexOfPhrase, StringComparison.Ordinal) + timingText.Length;
			}
			return lastIndexOfPhrase;
		}

		private void Update()
		{
			if (isActiveAndEnabled && isRunning)
			{
				if (IsStarted && !IsFinished)
				{
					if (!ReferenceEquals(CurrentTiming, previousTiming))
					{
						textEffect.OnTimingStarted(CurrentTiming);
						previousTiming = CurrentTiming;
					}
					
					var progress = (source.time - CurrentTiming.StartPosition) / CurrentTiming.Size;
					textEffect.OnTimingMoving(CurrentTiming, progress);

					if (OnWordReached == null) 
						return;
					
					var indexOfText = timings.Text.IndexOf(CurrentTiming.Text, StringComparison.Ordinal);
					var currentIndex = (int) (progress * CurrentTiming.Text.Length);
					currentIndex = Mathf.Clamp(currentIndex, 0, CurrentTiming.Text.Length - 1);
					var charIndex = indexOfText + currentIndex;
					for (var i = wordIndexReached; i < words.Count; i++)
					{
						var index = words[i];
						if (charIndex >= index.StartIndex && charIndex <= index.StartIndex + index.Length)
						{
							var word = timings.Text.Substring(index.StartIndex, index.Length);
							OnWordReached(word, index.StartIndex);
							wordIndexReached++;
							break;
						}
					}
				}
				else if (IsFinished)
				{
					OnFinish();
				}
			}
		}

		private void OnFinish()
		{
			textEffect.OnTimingMoving(CurrentTiming, 1f);
			textEffect.OnTimingFinished(CurrentTiming);
			if (currentSubTimingIndex == timings.Timings.Count - 1)
			{
				Stop();
				textEffect.OnEffectFinished();
				OnSyncFinished?.Invoke();
				return;
			}
			currentSubTimingIndex++;
			textEffect.OnTimingEntered(CurrentTiming);
		}
	}
}