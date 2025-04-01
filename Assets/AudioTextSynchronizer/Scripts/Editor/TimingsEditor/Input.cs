using System;
using UnityEngine;

namespace AudioTextSynchronizer.Editor.Timings
{
	public class Input
	{
		public Action OnMouseWheel;
		public Action<int> OnMouseDown;
		public Action<int> OnMouseDrag;
		public Action<int> OnMouseUp;
		public Action<int> OnRepaint;
		public Action<int> OnKeyDown;

		public void Update()
		{
			var controlId = GUIUtility.GetControlID(FocusType.Passive);
			switch (Event.current.GetTypeForControl(controlId))
			{
				case EventType.ScrollWheel:
					OnMouseWheel?.Invoke();
					break;
				case EventType.MouseDown:
					OnMouseDown?.Invoke(controlId);
					break;
				case EventType.MouseDrag:
					OnMouseDrag?.Invoke(controlId);
					break;
				case EventType.MouseUp:
					OnMouseUp?.Invoke(controlId);
					break;
				case EventType.Repaint:
					OnRepaint?.Invoke(controlId);
					break;
				case EventType.KeyDown:
					OnKeyDown?.Invoke(controlId);
					break;
			}
		}
	}
}