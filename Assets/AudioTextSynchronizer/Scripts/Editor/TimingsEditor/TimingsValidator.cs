using System;
using System.Linq;
using System.Collections.Generic;
using AudioTextSynchronizer.Core;
using UnityEngine;

namespace AudioTextSynchronizer.Editor.Timings
{
    public class TimingsValidator
    {
        private PhraseAsset phraseAsset;
        private readonly char[] indentsChar = {' ', '\t', '\n'};

        public void ValidateTimings(PhraseAsset data)
        {
            phraseAsset = data;
            var result = string.Empty;
            var timingsWithProblems = new List<int>();
            var indexOf = 0;
            for (var i = 0; i < phraseAsset.Timings.Count; i++)
            {
                var timingText = phraseAsset.Timings[i].Text;
                var indexOfTiming = phraseAsset.Text.IndexOf(timingText, indexOf, StringComparison.Ordinal);
                if (indexOfTiming != -1 && !string.IsNullOrEmpty(timingText))
                {
                    var postfix = string.Empty;
                    if (phraseAsset.Text.Length > indexOfTiming + 1 && indentsChar.Contains(phraseAsset.Text[indexOfTiming + 1]))
                    {
                        postfix = phraseAsset.Text[indexOfTiming + 1].ToString();
                    }
                    result += timingText + postfix;
                    indexOf = indexOfTiming + timingText.Length;
                }
                else
                {
                    timingsWithProblems.Add(i);
                    var prefix = string.Empty;
                    if (phraseAsset.Text.Length > indexOf + 1 && indentsChar.Contains(phraseAsset.Text[indexOf]))
                    {
                        prefix = phraseAsset.Text[indexOf].ToString();
                    }
                    var postfix = string.Empty;
                    var indexOfNextTiming = -1;
                    if (phraseAsset.Timings.Count > i + 1)
                    {
                        var increment = 1;
                        while (phraseAsset.Timings.Count > i + increment && indexOfNextTiming == -1 && phraseAsset.Text.Length > indexOf + prefix.Length + timingText.Length)
                        {
                            indexOfNextTiming = phraseAsset.Text.IndexOf(phraseAsset.Timings[i + increment].Text, indexOf + prefix.Length + timingText.Length, StringComparison.Ordinal);
                            var validIndex = indexOfNextTiming - 1 >= 0;
                            var validTextIndex = phraseAsset.Text.Length > indexOfNextTiming - 1;
                            if (indexOfNextTiming != -1 && validIndex && validTextIndex && indentsChar.Contains(phraseAsset.Text[indexOfNextTiming - 1]))
                            {
                                postfix = phraseAsset.Text[indexOfNextTiming - 1].ToString();
                            }
                            else
                            {
                                increment++;
                            }
                        }
                    }
                    
                    var phrase = prefix + "<b><color=red>" + timingText + "</color></b>" + postfix;
                    result += phrase;
                }
            }
            
            PrintResult(result, timingsWithProblems);
        }

        private void PrintResult(string result, ICollection<int> timingsWithProblem)
        {
            if (timingsWithProblem.Count > 0)
            {
                Debug.Log("Result: " + result);
                foreach (var index in timingsWithProblem)
                {
                    var expectedLength = phraseAsset.Timings[index].Text.Length;
                    var expectedText = string.Empty;
                    if (phraseAsset.Timings.Count > index + 1)
                    {
                        if (index > 0)
                        {
                            if (timingsWithProblem.Contains(index - 1) || timingsWithProblem.Contains(index + 1))
                            {
                                expectedLength = -1;
                            }
                            else
                            {
                                var startIndex = 0;
                                var firstTimingIndex = 0;
                                for (var i = 0; i < phraseAsset.Timings.Count; i++)
                                {
                                    if (i == index)
                                        break;
                                    startIndex = phraseAsset.Text.IndexOf(phraseAsset.Timings[i].Text, startIndex, StringComparison.Ordinal) + phraseAsset.Timings[i].Text.Length;
                                    firstTimingIndex = i;
                                }
                                var endIndex = phraseAsset.Text.IndexOf(phraseAsset.Timings[firstTimingIndex + 2].Text, startIndex, StringComparison.Ordinal);
                                expectedText = phraseAsset.Text.Substring(startIndex, endIndex - startIndex).Trim(indentsChar);
                                expectedLength = expectedText.Length;
                            }
                        }
                    }
                    else
                    {
                        var previousTiming = phraseAsset.Timings[phraseAsset.Timings.Count - 2];
                        var indexOffset = 0;
                        var lastIndex = phraseAsset.Text.LastIndexOf(previousTiming.Text, StringComparison.Ordinal);
                        if (lastIndex != -1 && phraseAsset.Text.Length > lastIndex + previousTiming.Text.Length)
                        {
                            if (indentsChar.Contains(phraseAsset.Text[lastIndex + previousTiming.Text.Length]))
                            {
                                indexOffset = 1;
                            }
                        }
                        var startIndexOfLastTiming = lastIndex + previousTiming.Text.Length + indexOffset;
                        expectedText = phraseAsset.Text.Substring(startIndexOfLastTiming, phraseAsset.Text.Length - startIndexOfLastTiming).Trim(indentsChar);
                        expectedLength = expectedText.Length;
                    }

                    if (string.IsNullOrEmpty(expectedText))
                    {
                        Debug.Log("Probably timing <b>" + phraseAsset.Timings[index].Name + "</b> is excess. Try to delete or modify it's text.");
                    }
                    else
                    {
                        Debug.Log("<b>Problem timing:</b> " + phraseAsset.Timings[index].Name +
                                  "\n<b>Text:</b> " + phraseAsset.Timings[index].Text +
                                  "\n<b>Expected text:</b> " + expectedText +
                                  "\n<b>Text length:</b> " + phraseAsset.Timings[index].Text.Length +
                                  "\n<b>Expected length:</b> " + expectedLength + Environment.NewLine);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(phraseAsset.Text))
                {
                    Debug.Log("Main text is empty!");

                }
                else if (phraseAsset.Timings.Count == 0)
                {
                    Debug.Log("Timings doesn't exists!");
                }
                else
                {
                    Debug.Log("Timings are valid!");
                }
            }
        }
    }
}