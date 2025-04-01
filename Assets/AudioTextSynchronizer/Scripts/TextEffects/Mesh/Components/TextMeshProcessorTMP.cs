using AudioTextSynchronizer.TextEffects.Components.Base;
#if TEXTMESHPRO_3_0_OR_NEWER
using System;
using System.Collections;
using UnityEngine;
using TMPro;
#endif

namespace AudioTextSynchronizer.TextEffects.Components
{
	public class TextMeshProcessorTMP : TextMeshProcessorBase
	{
		public override bool SkipIndents => false;
#if TEXTMESHPRO_3_0_OR_NEWER
		private TMP_Text textMeshPro;
		public TMP_Text TextMeshPro => textMeshPro;
		
		public override void PreInit()
		{
			if (textMeshPro == null)
			{
				TryGetComponent(out textMeshPro);
				Graphic = textMeshPro;
			}
			OnPreSetNewText();
		}
		
		public override void Init()
		{
			base.Init();
			IsStarted = false;
			StartCoroutine(WaitOneFrameAction(OnSetNewText));
		}

		private IEnumerator WaitOneFrameAction(Action action)
		{
			yield return null;
			action?.Invoke();
		}

		public override void ResetProgress()
		{
			base.ResetProgress();
			IsFinishedInternal = false;
			Animation.ResetProgress();
		}

		protected override void UpdateMesh()
		{
			if (!Application.isPlaying || !isActiveAndEnabled || !IsStarted || IsFinishedInternal)
				return;
			
			var textInfo = textMeshPro.textInfo;
			var mesh = textInfo.meshInfo[0];
			var characterCount = textInfo.characterCount;
			
			for (var i = 0; i < characterCount; i++)
			{
				if (!textInfo.characterInfo[i].isVisible)
					continue;

				var vertexIndex = textInfo.characterInfo[i].vertexIndex;
				for (var j = 0; j < 4; j++)
				{
					Animation.ProcessVertexBeforeEffect(textInfo, vertexIndex + j);
				}
			}

			var charsCount = CharsProgress.Count;
			for (var i = 0; i < charsCount; i++)
			{
				var charInfo = textInfo.characterInfo[i];
				if (!charInfo.isVisible)
					continue;

				var progress = CharsProgress[i];
				var vertexIndex = charInfo.vertexIndex;
				for (var j = 0; j < 4; j++)
				{
					Animation.ProcessVertexEffect(textInfo, vertexIndex, vertexIndex + j, progress);
				}
			}

			textMeshPro.mesh.vertices = mesh.vertices;
			textMeshPro.mesh.colors32 = mesh.colors32;
			
			for (var i = 0; i < textInfo.meshInfo.Length; i++)
			{
				textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
				textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
			}
		}

		public override void OnCharProgress(int charIndex)
		{
			if (FillInstantly)
			{
				if (CharsProgress.Count < textMeshPro.textInfo.characterCount)
				{
					var count = textMeshPro.textInfo.characterCount;
					for (var i = CharsProgress.Count; i < count; i++)
					{
						CharsProgress.Add(textMeshPro.textInfo.characterInfo[i].isVisible ? 0f : 1f);
					}
				}
			}
			else if (CharsProgress.Count < charIndex && CharsProgress.Count < textMeshPro.textInfo.characterCount)
			{
				var count = charIndex - CharsProgress.Count;
				for (var i = 0; i < count; i++)
				{
					CharsProgress.Add(textMeshPro.textInfo.characterInfo[CharsProgress.Count].isVisible ? 0f : 1f);
				}
			}
		}

		public override void OnPreSetNewText()
		{
			textMeshPro.canvasRenderer.SetAlpha(0f);
		}

		public override void OnSetNewText()
		{
			IsStarted = false;
			StartCoroutine(WaitOneFrameAction(() =>
			{
				IsStarted = true;
				UpdateInternal(true);
				textMeshPro.canvasRenderer.SetAlpha(1f);
			}));
		}

		public override void Dispose()
		{
			StopAllCoroutines();
			base.Dispose();
		}
#endif
	}
}