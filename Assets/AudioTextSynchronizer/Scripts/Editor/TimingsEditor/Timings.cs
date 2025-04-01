using System;
using System.Collections.Generic;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.Tools;
using UnityEngine;
using UnityEditor;

namespace AudioTextSynchronizer.Editor.Timings
{
	[Serializable]
	public class Timings
	{
		[SerializeField]
		private List<Timing> selectedTimings = new List<Timing>();
		private Timing dragSideTiming;
		private Vector2 mousePositionDown;
		private Vector2 timingTextScrollPosition;
		private float dragStartPosition;
		private float startShiftDownOffset;
		private bool shouldDragSide;
		private bool isDragLeft;
		private bool isKeyAltDown;
		private bool isDragStarted;

		public static Color DefaultColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 41 / 255f);
		private readonly Color timingLineColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 127f / 255f);
		private readonly Color selectedColor = new Color(118 / 255f, 176 / 255f, 244 / 255f, 127 / 255f);

		public void OnMouseUp(int controlId)
		{
			if (GUIUtility.hotControl == controlId)
			{
				GUIUtility.hotControl = 0;
				if (CanSelectTimings())
				{
					OnSelectionFinished(GetRectFromSelection());
				}
				isDragStarted = false;
				shouldDragSide = false;
				Event.current.Use();
			}
		}

		public void OnKeyDown(int controlId)
		{
			if (Event.current.keyCode == KeyCode.A && (Event.current.command || Event.current.alt))
			{
				var rect = TimingsWindow.instance.GetTimingsRect();
				if (rect.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
					SelectAllTimings();
					isDragStarted = false;
					shouldDragSide = false;
				}
			}
		}

		public void OnMouseDown(float height)
		{
			mousePositionDown = Event.current.mousePosition;
			var timings = TimingsWindow.instance.Data.Timings;
			if (!Event.current.alt && !Event.current.shift)
			{
				selectedTimings.Clear();
			}

			for (var i = timings.Count - 1; i >= 0; i--)
			{
				var timing = timings[i];
				var rectControlLeft = TimingsWindow.instance.GetPixelRect(timing.StartPosition);
				var rectControlRight = TimingsWindow.instance.GetPixelRect(timing.EndPosition);
				var cursorRect = new Rect(mousePositionDown.x, mousePositionDown.y, 3f, height);
				var overlapsLeft = rectControlLeft.Overlaps(cursorRect);
				var overlapsRight = rectControlRight.Overlaps(cursorRect);
				shouldDragSide = overlapsLeft || overlapsRight;
				isDragLeft = overlapsLeft;
				dragStartPosition = Event.current.mousePosition.x;
				if (overlapsLeft || overlapsRight)
				{
					if (!selectedTimings.Contains(timing))
					{
						selectedTimings.Add(timing);
					}

					dragSideTiming = timing;
					break;
				}

				var width = Mathf.Abs(rectControlRight.xMin - rectControlLeft.xMin);
				var timingHeight = rectControlRight.yMax - rectControlLeft.yMin;
				var timingRect = new Rect(rectControlLeft.xMin, rectControlLeft.yMin, width, timingHeight);
				if (timingRect.Contains(Event.current.mousePosition))
				{
					if (!selectedTimings.Contains(timing))
					{
						DeselectLayout();
						selectedTimings.Add(timing);
						break;
					}
				}
			}
		}

		private bool CanSelectTimings()
		{
			return !Event.current.alt && !Event.current.shift && !shouldDragSide;
		}

		public void DrawSelectionRect(int controlId)
		{
			if (controlId <= 0) 
				throw new ArgumentOutOfRangeException(nameof(controlId));
		
			if (GUIUtility.hotControl == controlId)
			{
				if (CanSelectTimings())
				{
					var rectSelection = GetRectFromSelection();
					if (rectSelection.size.magnitude > 0f)
					{
						GUI.skin.box.Draw(rectSelection, GUIContent.none, controlId);
					}
				}
			}
		}

		public void AddTiming(float start, float end)
		{
			var data = TimingsWindow.instance.Data;
			var timingName = "Timing " + (data.Timings.Count + 1);
			Undo.RecordObject(data, "Add Timing");
			data.Timings.Add(new Timing(start, end, DefaultColor, timingName));
		}

		public void SelectAllTimings()
		{
			var timings = TimingsWindow.instance.Data.Timings;
			if (timings != null && timings.Count > 0)
			{
				selectedTimings.Clear();
				foreach (var timing in timings)
				{
					DeselectLayout();
					selectedTimings.Add(timing);
				}
			}
		}

		public void SelectedLastTimingAdded(float xPosition)
		{
			var timings = TimingsWindow.instance.Data.Timings;
			if (timings != null && timings.Count > 0)
			{
				dragStartPosition = xPosition;
				var timing = timings[timings.Count - 1];
				if (!selectedTimings.Contains(timing))
				{
					DeselectLayout();
					selectedTimings.Add(timing);
				}
			}
		}

		public List<Timing> GetSelectedTimings()
		{
			return selectedTimings;
		}

		public int GetSelectedTimingsCount()
		{
			return selectedTimings.Count;
		}

		public void Deselect()
		{
			selectedTimings.Clear();
		}

		public Timing GetFirstSelectedTiming()
		{
			if (selectedTimings.Count > 0)
			{
				return selectedTimings[0];
			}

			return null;
		}

		private void DeselectLayout()
		{
			GUI.SetNextControlName(string.Empty);
			GUI.FocusControl(string.Empty);
		}

		private void OnSelectionFinished(Rect selectedArea)
		{
			if (selectedArea.size == Vector2.zero) 
				return;
			
			var timings = TimingsWindow.instance.Data.Timings;
			foreach (var timing in timings)
			{
				var overlaps = selectedArea.Overlaps(timing.Rectangle);
				if (overlaps && !selectedTimings.Contains(timing))
				{
					selectedTimings.Add(timing);
				}
				else if (!overlaps)
				{
					selectedTimings.Remove(timing);
				}
			}
		}

		private Rect GetRectFromSelection()
		{
			var rect = new Rect(mousePositionDown.x, mousePositionDown.y,
				Event.current.mousePosition.x - mousePositionDown.x,
				Event.current.mousePosition.y - mousePositionDown.y);
			var panelHeight = TimingsWindow.instance.GetPanelHeight();
			if (mousePositionDown.y + rect.height < EditorStyles.toolbar.fixedHeight)
			{
				rect.height = -mousePositionDown.y + EditorStyles.toolbar.fixedHeight;
			}
			if (rect.height > panelHeight - mousePositionDown.y)
			{
				rect.height = panelHeight - mousePositionDown.y;
			}
			if (rect.width < 0)
			{
				rect.x += rect.width;
				rect.width = -rect.width;
			}
			if (rect.height < 0)
			{
				rect.y += rect.height;
				rect.height = -rect.height;
			}
			return rect;
		}

		public void OnDragTiming(float width, float paddingLeft)
		{
			if (selectedTimings.Count > 0)
			{
				var data = TimingsWindow.instance.Data;
				var shouldDragCurrentTiming = Event.current.shift;
				var shouldDragCurrentTimingSlowly = Event.current.alt;
				var mousePosX = Event.current.mousePosition.x;
				if ((!isDragStarted || !isKeyAltDown) && shouldDragCurrentTimingSlowly)
				{
					isDragStarted = true;
					startShiftDownOffset = mousePosX;
				}
				isKeyAltDown = shouldDragCurrentTimingSlowly;
				var dragSpeed = shouldDragCurrentTimingSlowly ? 0.9f : 0f;
				var dragVector = mousePosX - startShiftDownOffset;
				var offsetDrag = dragVector * dragSpeed;
				var positionX = mousePosX - paddingLeft;
				var moveSpeed = shouldDragCurrentTimingSlowly ? 0.1f : 1f;
				var moveVector = mousePosX - dragStartPosition;
				var ratio = data.Clip.length / width;
				var dragPositionInSeconds = (positionX - offsetDrag) * ratio;
				var movePositionInSeconds = moveVector * moveSpeed * ratio;

				foreach (var timing in selectedTimings)
				{
					if (dragPositionInSeconds < 0f)
					{
						dragPositionInSeconds = 0f;
					}

					if (dragPositionInSeconds > data.Clip.length)
					{
						dragPositionInSeconds = data.Clip.length;
					}

					if (shouldDragSide && Equals(dragSideTiming, timing))
					{
						if (isDragLeft)
						{
							timing.StartPosition = dragPositionInSeconds;
							if (timing.StartPosition > timing.EndPosition)
							{
								ExtendedMethods.Swap(ref timing.StartPosition, ref timing.EndPosition);
								isDragLeft = !isDragLeft;
							}
						}
						else
						{
							timing.EndPosition = dragPositionInSeconds;
							if (timing.EndPosition < timing.StartPosition)
							{
								ExtendedMethods.Swap(ref timing.StartPosition, ref timing.EndPosition);
								isDragLeft = !isDragLeft;
							}
						}
					}
					else if (shouldDragCurrentTiming || shouldDragCurrentTimingSlowly)
					{
						var isLeftBorderReached = timing.StartPosition + movePositionInSeconds < 0f;
						var isRightBorderReached = timing.EndPosition + movePositionInSeconds >= data.Clip.length;
						if (isLeftBorderReached || isRightBorderReached)
						{
							movePositionInSeconds = isLeftBorderReached ? -timing.StartPosition : data.Clip.length - timing.EndPosition;
							dragStartPosition = startShiftDownOffset = Event.current.mousePosition.x;
							break;
						}
					}
				}

				if (!shouldDragSide && (shouldDragCurrentTiming || shouldDragCurrentTimingSlowly))
				{
					foreach (var timing in selectedTimings)
					{
						timing.StartPosition += movePositionInSeconds;
						timing.EndPosition += movePositionInSeconds;
					}
					EditorUtility.SetDirty(TimingsWindow.instance.Data);
				}
				dragStartPosition = Event.current.mousePosition.x;
			}
		}

		public void Draw()
		{
			var timings = TimingsWindow.instance.Data.Timings;
			var audioCurveRect = TimingsWindow.instance.GetCurveRect();
			var audioCurveWidth = audioCurveRect.width;
			var audioPanelHeight = TimingsWindow.instance.GetPanelHeight();
			var audioCurvePaddingLeft = audioCurveRect.xMin;
			Handles.BeginGUI();
			foreach (var timing in timings)
			{
				var startValue = GetPixelPosition(timing.StartPosition, audioCurveWidth, audioCurvePaddingLeft);
				var endValue = GetPixelPosition(timing.EndPosition, audioCurveWidth, audioCurvePaddingLeft);
				var leftBorder = GetPixelRect(startValue, audioPanelHeight);
				var rightBorder = GetPixelRect(endValue, audioPanelHeight);
				var timingWidth = rightBorder.xMin - leftBorder.xMin;
				var timingHeight = rightBorder.yMax - leftBorder.yMin;
				var rect = new Rect(leftBorder.xMin, leftBorder.yMin, timingWidth, timingHeight);
				timing.Rectangle = rect;
				EditorGUI.DrawRect(timing.Rectangle, selectedTimings.Contains(timing) ? selectedColor : timing.GetColor());
				DrawTimingLine(leftBorder);
				DrawTimingLine(rightBorder);
				var textRect = rect;
				textRect.height -= TimingsWindow.ScrollHeight;
				var testStyle = EditorStyles.whiteMiniLabel;
				testStyle.alignment = TextAnchor.MiddleCenter;
				EditorGUI.LabelField(textRect, timing.Name, testStyle);
			}
			Handles.EndGUI();
		}

		private float GetPixelPosition(float pos, float width, float padding)
		{
			return pos / TimingsWindow.instance.Data.Clip.length * (width) + padding;
		}

		private Rect GetPixelRect(float pos, float height)
		{
			return new Rect(pos, EditorStyles.toolbar.fixedHeight, 3f, height);
		}

		public float SecondsToTimeline(float time, float width)
		{
			var timelinePosition = time / TimingsWindow.instance.Data.Clip.length * width * width;
			return timelinePosition;
		}

		private void DrawTimingLine(Rect rect)
		{
			EditorGUI.DrawRect(rect, timingLineColor);
			EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
		}

		public void DrawInfo()
		{
			var selectedTiming = GetFirstSelectedTiming();
			if (selectedTiming == null || GetSelectedTimingsCount() > 1) 
				return;

			if (TimingsWindow.instance != null && TimingsWindow.instance.Data != null)
			{
				Undo.RecordObject(TimingsWindow.instance.Data, "Timing Data Changed");
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Timing name: ", EditorStyles.label, GUILayout.Width(TimingsWindow.LabelWidth));
			selectedTiming.Name = EditorGUILayout.TextField(selectedTiming.Name, EditorStyles.textField);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			var leftTime = ExtendedMethods.FormatTimeSpan(TimeSpan.FromSeconds(selectedTiming.StartPosition), true);
			EditorGUILayout.LabelField("Start: " + leftTime, EditorStyles.label);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			var rightTime = ExtendedMethods.FormatTimeSpan(TimeSpan.FromSeconds(selectedTiming.EndPosition), true);
			EditorGUILayout.LabelField("End: " + rightTime, EditorStyles.label);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			var durationValue = ExtendedMethods.FormatTimeSpan(TimeSpan.FromSeconds(selectedTiming.Size), true);
			EditorGUILayout.LabelField("Duration: " + durationValue, EditorStyles.label);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Color: ", EditorStyles.label, GUILayout.Width(TimingsWindow.LabelWidth));
			var color = selectedTiming.Color;
			var newColor = EditorGUILayout.ColorField(selectedTiming.Color);
			if (newColor != color)
			{
				selectedTiming.SetColor(newColor);
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Timing text: ", EditorStyles.label, GUILayout.Width(TimingsWindow.LabelWidth));
			var numLines = selectedTiming.Text.Length - selectedTiming.Text.Replace(Environment.NewLine, string.Empty).Length;
			var showScroll = numLines > 3;
			if (showScroll)
			{
				numLines = Mathf.Min(numLines, numLines, 4);
				var height = EditorGUIUtility.singleLineHeight - 2;
				timingTextScrollPosition = EditorGUILayout.BeginScrollView(timingTextScrollPosition, GUILayout.Height(height * numLines));
			}
			selectedTiming.Text = EditorGUILayout.TextArea(selectedTiming.Text, EditorStyles.textArea);
			if (showScroll)
			{
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}