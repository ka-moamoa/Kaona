using System.Collections.Generic;
using AudioTextSynchronizer.TextEffects.MeshAnimations.Base;
using UnityEngine;
#if TEXTMESHPRO_3_0_OR_NEWER
using TMPro;
#endif

namespace AudioTextSynchronizer.TextEffects.MeshAnimations
{
	[CreateAssetMenu(fileName = nameof(AlphaColorMeshAnimation), menuName = "Audio Text Synchronizer/Text Effects/Mesh/Mesh Animations/" + nameof(AlphaColorMeshAnimation))]
	public class AlphaColorMeshAnimation : MeshAnimationBase
	{
		public bool UseAlpha = true;
		public AnimationCurve AlphaCurve = AnimationCurve.Linear(0, 0, 1, 1);
		public Color32 TextColor;
		private readonly List<Color32> verticesColor = new List<Color32>();
		private Color32? startColor;
		private readonly Color32 transparentColor = new Color32(0, 0, 0, 0);

		public override void ResetProgress()
		{
			verticesColor.Clear();
		}

		#region TextMeshPro
#if TEXTMESHPRO_3_0_OR_NEWER
		public override void ProcessVertexBeforeEffect(TMP_TextInfo textInfo, int vertexIndex)
		{
			var meshInfo = textInfo.meshInfo[0];
			if (!startColor.HasValue)
			{
				startColor = textInfo.meshInfo[0].colors32[vertexIndex];
			}
			
			if (verticesColor.Count == 0)
			{
				for (var i = 0; i < meshInfo.vertices.Length; i += 4)
				{
					for (var j = 0; j < 4; j++)
					{
						verticesColor.Add(TextColor);
					}
				}
			}
			
			meshInfo.colors32[vertexIndex] = transparentColor;
		}

		public override void ProcessVertexEffect(TMP_TextInfo textInfo, int startVertexIndex, int vertexIndex, float progress)
		{
			var color = verticesColor[vertexIndex];
			var alpha = UseAlpha ? (byte)Mathf.Clamp(startColor.Value.a * AlphaCurve.Evaluate(progress), 0, 255) : (byte)255;
			textInfo.meshInfo[0].colors32[vertexIndex] = new Color32(color.r, color.g, color.b, alpha);
		}
#endif
		#endregion

		#region UI.Text

		public override UIVertex ProcessVertexBeforeEffect(List<UIVertex> vertices, UIVertex vertex)
		{
			if (verticesColor.Count == 0)
			{
				for (var i = 0; i < vertices.Count; i += 6)
				{
					for (var j = 0; j < 6; j++)
					{
						verticesColor.Add(TextColor);
					}
				}
			}
		
			vertex.color = transparentColor;
			return vertex;
		}

		public override UIVertex ProcessVertexEffect(UIVertex vertex, int vertexIndex, float progress)
		{
			var color = verticesColor[vertexIndex];
			var alpha = UseAlpha ? (byte)Mathf.Clamp(color.a * AlphaCurve.Evaluate(progress), 0, 255) : (byte)255;
			vertex.color = new Color32(color.r, color.g, color.b, alpha);
			return vertex;
		}
		
		#endregion
	}
}