using System;
using System.Linq;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.TextEffects;
using AudioTextSynchronizer.TextEffects.Components.Base;
using AudioTextSynchronizer.TextEffects.MeshAnimations.Base;
using AudioTextSynchronizer.TextHighlighters.Base;
using AudioTextSynchronizer.TextSplitters.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AudioTextSynchronizer.Demo
{
	public class Demo : MonoBehaviour
	{
		[Serializable]
		public class PhraseData
		{
			public string Name;
			public PhraseAsset PhraseAsset;
		}

		[SerializeField] private TextSynchronizer textSynchronizer;

		[Header("Text Effects")]
		[SerializeField] private MeshTextEffect meshTextEffect;
		[SerializeField] private RichTextEffect richTextEffect;
		
		[Header("Text Splitters")]
		[SerializeField] private TextSplitConfigBase defaultTextSplitConfig;
		[SerializeField] private TextSplitConfigBase stringTextSplitConfig;
		[SerializeField] private TextSplitConfigBase timingsTextSplitConfig;

		[Header("Mesh Animations")]
		[SerializeField] private MeshAnimationBase alphaCenterAnimation;
		[SerializeField] private MeshAnimationBase alphaColorAnimation;
		
		[Header("Rich Highlight Configs")]
		[SerializeField] private TextHighlightConfigBase defaultHighlightConfig;
		[SerializeField] private TextHighlightConfigBase stringHighlightConfig;
		[SerializeField] private TextHighlightConfigBase subTimingHighlightConfig;

		[Header("Buttons")]
		[SerializeField] private Button playPauseButton;
		[SerializeField] private Button restartButton;
		[SerializeField] private Button playMeshEffectWithAlphaCenterAnimation;
		[SerializeField] private Button playMeshEffectWithAlphaAnimation;
		[SerializeField] private Button playKaraokeEffect;
		[SerializeField] private Button playRichEffect;
			
		[Header("Other")]
		[SerializeField] private Toggle togglePartialAppearing;
		[SerializeField] private Toggle togglePhraseAppearing;
		[SerializeField] private Toggle toggleFillInstantly;
		[SerializeField] private Dropdown timingsDropdown;
		[SerializeField] private PhraseData[] phrases;
		[SerializeField] private Transform[] reels;
		[SerializeField] private float reelSpeed = 10f;

		private Action lastButtonAction;
		private MeshTextEffect meshEffect;
		private RichTextEffect richEffect;
		private bool partialAppearing;
		private bool phraseAppearing;
		private bool fillTextInstantly;

		private void Awake()
		{
#if ATS_DEBUG
			Application.runInBackground = false;
#endif
#if !UNITY_WEBGL
			Application.targetFrameRate = Mathf.Clamp(Screen.currentResolution.refreshRate, 30, Screen.currentResolution.refreshRate);
#endif
			meshEffect = Instantiate(meshTextEffect);
			richEffect = Instantiate(richTextEffect);
			
			playPauseButton.onClick.AddListener(PlayPause);
			restartButton.onClick.AddListener(Restart);
			playMeshEffectWithAlphaCenterAnimation.onClick.AddListener(PlayMeshEffectWithAlphaCenterAnimation);
			playMeshEffectWithAlphaAnimation.onClick.AddListener(PlayMeshEffectWithAlphaAnimation);
			playKaraokeEffect.onClick.AddListener(PlayKaraokeEffect);
			playRichEffect.onClick.AddListener(PlayRichEffect);

			togglePartialAppearing.onValueChanged.AddListener(OnPartialAppearing);
			togglePhraseAppearing.onValueChanged.AddListener(OnPhraseAppearing);
			toggleFillInstantly.onValueChanged.AddListener(OnFillInstantly);
			timingsDropdown.onValueChanged.AddListener(OnDropdown);
			
			PlayMeshEffectWithAlphaCenterAnimation();
		}

		private void OnDestroy()
		{
			Destroy(meshEffect);
			Destroy(richEffect);
			
			playPauseButton.onClick.RemoveListener(PlayPause);
			restartButton.onClick.RemoveListener(Restart);
			playMeshEffectWithAlphaCenterAnimation.onClick.RemoveListener(PlayMeshEffectWithAlphaCenterAnimation);
			playMeshEffectWithAlphaAnimation.onClick.RemoveListener(PlayMeshEffectWithAlphaAnimation);
			playKaraokeEffect.onClick.RemoveListener(PlayKaraokeEffect);
			playRichEffect.onClick.RemoveListener(PlayRichEffect);
			
			togglePartialAppearing.onValueChanged.RemoveListener(OnPartialAppearing);
			togglePhraseAppearing.onValueChanged.RemoveListener(OnPhraseAppearing);
			toggleFillInstantly.onValueChanged.RemoveListener(OnFillInstantly);
			timingsDropdown.onValueChanged.RemoveListener(OnDropdown);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				return;
			}

			if (Input.GetKeyDown(KeyCode.S))
			{
				textSynchronizer.SkipPhrase();
			}
			
			if (textSynchronizer.Source.isPlaying)
			{
				foreach (var spin in reels)
				{
					spin.Rotate(-spin.forward, reelSpeed * Time.deltaTime);
				}
			}
		}

		private void PlayPause()
		{
			if (textSynchronizer.Source.isPlaying)
			{
				textSynchronizer.Pause();
			}
			else
			{
				if (lastButtonAction == null)
				{
					PlayMeshEffectWithAlphaCenterAnimation();
				}
				else if (!textSynchronizer.IsRunning && textSynchronizer.Source.time == 0f)
				{
					//if sync is finished
					lastButtonAction();
					return;
				}
				textSynchronizer.Play();
			}
		}

		private void Restart()
		{
			if (lastButtonAction == null)
			{
				PlayMeshEffectWithAlphaCenterAnimation();
			}
			else
			{
				lastButtonAction();
			}
		}

		private void PlayMeshEffectWithAlphaCenterAnimation()
		{
			var textSplitter = defaultTextSplitConfig;
			if (partialAppearing)
			{
				textSplitter = stringTextSplitConfig;
			}
			else if (phraseAppearing)
			{
				textSplitter = timingsTextSplitConfig;
			}
			PlayMeshEffect(textSplitter, alphaCenterAnimation);
			lastButtonAction = PlayMeshEffectWithAlphaCenterAnimation;
		}

		private void PlayMeshEffectWithAlphaAnimation()
		{
			var textSplitter = defaultTextSplitConfig;
			if (partialAppearing)
			{
				textSplitter = stringTextSplitConfig;
			}
			else if (phraseAppearing)
			{
				textSplitter = timingsTextSplitConfig;
			}
			PlayMeshEffect(textSplitter, alphaColorAnimation);
			lastButtonAction = PlayMeshEffectWithAlphaAnimation;
		}

		private void PlayKaraokeEffect()
		{
			PlayRichEffect(timingsTextSplitConfig, defaultHighlightConfig);
			lastButtonAction = PlayKaraokeEffect;
		}

		private void PlayRichEffect()
		{
			var textSplitter = defaultTextSplitConfig;
			if (partialAppearing)
			{
				textSplitter = stringTextSplitConfig;
			}
			else if (phraseAppearing)
			{
				textSplitter = timingsTextSplitConfig;
			}
			PlayRichEffect(textSplitter, defaultHighlightConfig);
			lastButtonAction = PlayRichEffect;
		}

		private void PlayMeshEffect(TextSplitConfigBase textSplitConfig, MeshAnimationBase meshAnimation)
		{
			if (textSynchronizer.TextComponent.TryGetComponent<TextMeshProcessorBase>(out var textMeshProcessor))
			{
				textMeshProcessor.enabled = true;
			}
			meshEffect.MeshAnimation = meshAnimation;
			meshEffect.AnimateTextInstantly = fillTextInstantly;
			meshEffect.TextSplitConfig = textSplitConfig;
			textSynchronizer.TextEffect = meshEffect;
			StartSync();
		}
		
		private void PlayRichEffect(TextSplitConfigBase textSplitConfig, TextHighlightConfigBase highlightConfig)
		{
			if (textSynchronizer.TextComponent.TryGetComponent<TextMeshProcessorBase>(out var textMeshProcessor))
			{
				textMeshProcessor.enabled = false;
			}
			richEffect.TextSplitConfig = textSplitConfig;
			richEffect.TextHighlightConfig = highlightConfig;
			textSynchronizer.TextEffect = richEffect;
			StartSync();
		}
		
		private void OnPartialAppearing(bool partial)
		{
			partialAppearing = partial;
			toggleFillInstantly.interactable = togglePartialAppearing.isOn || togglePhraseAppearing.isOn;
		}

		private void OnPhraseAppearing(bool setText)
		{
			phraseAppearing = setText;
			toggleFillInstantly.interactable = togglePartialAppearing.isOn || togglePhraseAppearing.isOn;
		}

		private void OnFillInstantly(bool fill)
		{
			fillTextInstantly = fill;
		}

		private void OnDropdown(int id)
		{
			var key = timingsDropdown.options[id].text;
			var phraseAsset = phrases.First(x => x.Name == key).PhraseAsset;
			textSynchronizer.Stop();
			textSynchronizer.Timings = phraseAsset;
			textSynchronizer.Source.clip = phraseAsset.Clip;
			Restart();
		}

		private void StartSync()
		{
			textSynchronizer.Stop();
			textSynchronizer.Play(true);
		}
	}
}