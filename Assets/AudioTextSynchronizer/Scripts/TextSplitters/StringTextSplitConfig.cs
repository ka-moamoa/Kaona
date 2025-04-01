using System;
using System.Collections.Generic;
using AudioTextSynchronizer.Core;
using AudioTextSynchronizer.TextEffects.Base;
using AudioTextSynchronizer.TextSplitters.Base;
using UnityEngine;

namespace AudioTextSynchronizer.TextSplitters
{
	[CreateAssetMenu(fileName = nameof(StringTextSplitConfig), menuName = "Audio Text Synchronizer/Text Splitters/" + nameof(StringTextSplitConfig))]
	public class StringTextSplitConfig : TextSplitConfigBase
	{
		public string[] SplitTextByStrings = { "\n", "\r\n", "." };
		public bool TrimText = true;
		public char[] TrimCharacters = { ' ', '\t', '\n', '.' };

		public override void Init(PhraseAsset phraseAsset)
		{
			base.Init(phraseAsset);
			if (TextParts.Count == 0)
			{
				Debug.LogWarning("TextsParts is empty!");
			}
		}

		protected override void DivideTextForParts()
		{
			var positions = new List<int> { 0 };
			var mainText = Phrases.Text.Trim();
			foreach (var separator in SplitTextByStrings)
			{
				var previousStartIndex = -1;
				var startIndex = 0;
				do
				{
					if (startIndex != 0)
					{
						previousStartIndex = startIndex;
					}
					startIndex = mainText.IndexOf(separator, startIndex, StringComparison.Ordinal);
					if (startIndex != -1 && !positions.Contains(startIndex + separator.Length))
					{
						startIndex += separator.Length;
						positions.Add(startIndex);
					}
				} while (startIndex != -1 && startIndex != previousStartIndex);
			}

			positions.Sort();
			for (var i = 0; i < positions.Count; i++)
			{
				int length;
				var startIndex = positions[i];
				int endIndex;
				if (i == positions.Count - 1)
				{
					length = mainText.Length - startIndex;
				}
				else
				{
					endIndex = positions[i + 1];
					length = endIndex - startIndex;
				}

				string text;
				if (TrimText)
				{
					text = mainText.Substring(startIndex, length);
					var textStart = text.TrimStart(TrimCharacters);
					if (text.Length != textStart.Length)
					{
						var offset = text.Length - textStart.Length;
						positions[i] += offset;
					}

					var textEnd = text.TrimEnd(TrimCharacters);
					if (text.Length != textEnd.Length)
					{
						var offset = text.Length - textEnd.Length;
						if (positions.Count > i + 1)
						{
							positions[i + 1] -= offset;
						}
					}
				}

				startIndex = positions[i];
				if (i == positions.Count - 1)
				{
					endIndex = mainText.Length - 1;
					length = mainText.Length - startIndex;
				}
				else
				{
					endIndex = positions[i + 1];
					length = endIndex - startIndex;
				}

				if (startIndex >= endIndex)
					continue;

				text = mainText.Substring(startIndex, length);
				var timingIndex = Parts.Count > 0 ? GetTimingIndex(text) : 0;
				Parts.Add(new TextPart
				{
					Text = text,
					StartIndex = startIndex,
					StartsFromTimingIndex = timingIndex,
					EndIndex = endIndex
				});
			}
		}

		private int GetTimingIndex(string text)
		{
			var indexOf = 0;
			for (var i = 0; i < Phrases.Timings.Count; i++)
			{
				var timing = Phrases.Timings[i];
				indexOf = Mathf.Clamp(indexOf, 0, int.MaxValue);
				if (TrimText)
				{
					var trimmedText = timing.Text.Trim(TrimCharacters);
					indexOf = text.IndexOf(trimmedText, indexOf, StringComparison.Ordinal);
				}
				else
				{
					indexOf = text.IndexOf(timing.Text, indexOf, StringComparison.Ordinal);
				}

				if (indexOf != -1)
				{
					return i;
				}
			}
			return 0;
		}

		public override int GetNextTimingIndex(int currentTextPartsIndex, int currentTimingIndex)
		{
			if (currentTextPartsIndex + 1 < TextParts.Count)
			{
				return TextParts[currentTextPartsIndex + 1].StartsFromTimingIndex;
			}
			return currentTimingIndex;
		}
	}
}
