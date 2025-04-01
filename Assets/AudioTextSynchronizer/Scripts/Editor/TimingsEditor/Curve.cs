using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AudioTextSynchronizer.Editor.Timings
{
	[Serializable]
	public class Curve
	{
		[SerializeField] private float[] audioCurveData;
		private readonly Color proColor = new Color(255 / 255f, 168 / 255f, 7 / 255f);
		private readonly Color defaultColor = new Color(215 / 255f, 149 / 255f, 20 / 255f);
		private const float ScaleY = 0.5f;

		public void LoadData(AudioClip clip)
		{
			if (clip == null) 
				return;
			
			var clipPath = AssetDatabase.GetAssetPath(clip);
			var importer = AssetImporter.GetAtPath(clipPath);
			var assembly = Assembly.GetAssembly(typeof(AssetImporter));
			var type = assembly.GetType("UnityEditor.AudioUtil");
			var audioUtilGetMinMaxData = type.GetMethod("GetMinMaxData");
			audioCurveData = audioUtilGetMinMaxData.Invoke(null, new object[] {importer}) as float[];
		}

		public void Render()
		{
			if (Event.current.type == EventType.Repaint && audioCurveData != null)
			{
				var curveRect = TimingsWindow.instance.GetCurveRect();
				GL.PushMatrix();
				GL.LoadPixelMatrix();
				GL.Begin(GL.QUADS);
				GL.Color(EditorGUIUtility.isProSkin ? proColor : defaultColor);
				for (var i = 0; i < audioCurveData.Length; i += 4)
				{
					var y1 = curveRect.yMin + curveRect.height / 2f + audioCurveData[i + 0] * -curveRect.height * ScaleY;
					var y2 = curveRect.yMin + curveRect.height / 2f + audioCurveData[i + 1] * -curveRect.height * ScaleY;
					var y3 = curveRect.yMin + curveRect.height / 2f + audioCurveData[i + 2] * -curveRect.height * ScaleY;
					var y4 = curveRect.yMin + curveRect.height / 2f + audioCurveData[i + 3] * -curveRect.height * ScaleY;
					var x1 = curveRect.xMin + (i / 4 + 0) / (audioCurveData.Length / curveRect.width) * 4f;
					var x2 = curveRect.xMin + (i / 4 + 1) / (audioCurveData.Length / curveRect.width) * 4f;

					var v0 = new Vector3(x1, y2, 0);
					var v1 = new Vector3(x1, y1, 0);
					var v2 = new Vector3(x2, y3, 0);
					var v3 = new Vector3(x2, y4, 0);

					GL.Vertex(v0);
					GL.Vertex(v1);
					GL.Vertex(v2);
					GL.Vertex(v3);
				}
				GL.End();
				GL.PopMatrix();
			}
		}
	}
}