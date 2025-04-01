using System.Collections.Generic;
using AudioTextSynchronizer.TextEffects.MeshAnimations.Base;
using UnityEngine;
#if TEXTMESHPRO_3_0_OR_NEWER
using TMPro;
#endif

namespace AudioTextSynchronizer.TextEffects.MeshAnimations
{
	[CreateAssetMenu(fileName = nameof(AlphaCharScaleMeshAnimation), menuName = "Audio Text Synchronizer/Text Effects/Mesh/Mesh Animations/" + nameof(AlphaCharScaleMeshAnimation))]
	public class AlphaCharScaleMeshAnimation : MeshAnimationBase
	{
		public bool UseAlpha = true;
		public AnimationCurve AlphaCurve = AnimationCurve.Linear(0, 0, 1, 1);
		public AnimationCurve VerticesCurve = AnimationCurve.Linear(0, 0, 1, 1);

		private readonly List<Vector3> verticesCenter = new List<Vector3>();
		private readonly List<Vector3> verticesStart = new List<Vector3>();
		private Color32 startColor;
		private readonly Color32 transparentColor = new Color32(0, 0, 0, 0);

		public override void ResetProgress()
		{
			verticesCenter.Clear();
			verticesStart.Clear();
		}

		#region TextMeshPro
#if TEXTMESHPRO_3_0_OR_NEWER

		public override void ProcessVertexBeforeEffect(TMP_TextInfo textInfo, int vertexIndex)
		{
			var meshInfo = textInfo.meshInfo[0];
			if (verticesCenter.Count == 0)
			{
				startColor = textInfo.meshInfo[0].colors32[vertexIndex];
				for (var i = 0; i < textInfo.characterCount; i++)
				{
					if (!textInfo.characterInfo[i].isVisible)
						continue;
					
					var vertexCenter = Vector3.zero;
					var currentVertexIndex = textInfo.characterInfo[i].vertexIndex;
					for (var j = 0; j < 4; j++)
					{
						var currentVertex = meshInfo.vertices[currentVertexIndex + j];
						vertexCenter += currentVertex;
						verticesStart.Add(currentVertex);
					}
					vertexCenter /= 4f;
					verticesCenter.Add(vertexCenter);
				}
			}
			
			meshInfo.colors32[vertexIndex] = transparentColor;
		}

		public override void ProcessVertexEffect(TMP_TextInfo textInfo, int startVertexIndex, int vertexIndex, float progress)
		{
			var center = verticesCenter[startVertexIndex / 4];
			var vertexStart = verticesStart[vertexIndex];
			var endPositionRatio = VerticesCurve.Evaluate(progress);
			var position = Vector3.Lerp((textInfo.meshInfo[0].vertices[vertexIndex] - center) * endPositionRatio + center, vertexStart, progress);
			textInfo.meshInfo[0].vertices[vertexIndex] = position;
			var alpha = UseAlpha ? (byte)Mathf.Clamp(startColor.a * AlphaCurve.Evaluate(progress), 0, 255) : (byte)255;
			textInfo.meshInfo[0].colors32[vertexIndex] = new Color32(startColor.r, startColor.g, startColor.b, alpha);
		}
		
#endif
		#endregion

		#region UI.Text
		
		public override UIVertex ProcessVertexBeforeEffect(List<UIVertex> vertices, UIVertex vertex)
		{
			if (verticesCenter.Count == 0)
			{
				startColor = vertex.color;
				for (var i = 0; i < vertices.Count; i += 6)
				{
					var vertexPosition = Vector3.zero;
					for (var j = 0; j < 6; j++)
					{
						var currentVertex = vertices[i + j].position;
						vertexPosition += currentVertex;
						verticesStart.Add(currentVertex);
					}
					vertexPosition /= 6;
					verticesCenter.Add(vertexPosition);
				}
			}

			vertex.color = transparentColor;
			return vertex;
		}
		
		public override UIVertex ProcessVertexEffect(UIVertex vertex, int vertexIndex, float progress)
		{
			if (verticesCenter.Count > vertexIndex / 6)
			{
				var center = verticesCenter[vertexIndex / 6];
				var vertexStart = verticesStart[vertexIndex];
				var endPositionRatio = VerticesCurve.Evaluate(progress);
				vertex.position = Vector3.Lerp((vertex.position - center) * endPositionRatio + center, vertexStart, progress);
				var alpha = UseAlpha ? (byte)Mathf.Clamp(startColor.a * AlphaCurve.Evaluate(progress), 0, 255) : (byte)255;
				vertex.color = new Color32(startColor.r, startColor.g, startColor.b, alpha);
			}
			return vertex;
		}
		
		#endregion
	}
}