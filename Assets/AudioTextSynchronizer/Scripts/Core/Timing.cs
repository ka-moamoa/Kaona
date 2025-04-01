using System;
using UnityEngine;

namespace AudioTextSynchronizer.Core
{
	[Serializable]
	public class Timing
	{
		public Timing(float start, float end, Color color, string name = "", string text = "")
		{
			StartPosition = start;
			EndPosition = end;
			Color = color;
			Name = name;
			Text = text;
		}

		public float StartPosition;
		public float EndPosition;
		public string Name;
		[Multiline] public string Text;
		[HideInInspector] public Rect Rectangle;
		[HideInInspector] public Color Color;
		
		public float Size => Mathf.Abs(StartPosition - EndPosition);

		public void SetColor(Color color)
		{
			Color = color;
		}

		public Color GetColor()
		{
			return Color;
		}

		public override bool Equals(object obj)
		{
			var timing = (Timing)obj;
			return timing != null && Color == timing.Color && Name == timing.Name && Rectangle == timing.Rectangle &&
			       Text == timing.Text && EndPosition == timing.EndPosition && StartPosition == timing.StartPosition;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}