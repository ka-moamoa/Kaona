using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AudioTextSynchronizer.Tools
{
    public static class ExtendedMethods
    {
        public static void Swap<T>(ref T left, ref T right)
        {
            var temp = left;
            left = right;
            right = temp;
        }
        
        public static string ColorToHex(this Color32 color)
        {
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }

        public static string FormatTimeSpan(TimeSpan newTime, bool formatMilliseconds = false)
        {
            string formattedTime;
            if (newTime.Hours > 0)
            {
                if (formatMilliseconds)
                {
                    formattedTime = string.Format("{0}:{1:D2}:{2:D2}", newTime.Hours, newTime.Minutes, newTime.Seconds);
                }
                else
                {
                    formattedTime = string.Format("{0}:{1:D2}:{2:D2}:{3:D2}", newTime.Hours, newTime.Minutes,
                        newTime.Seconds, newTime.Milliseconds);
                }
            }
            else
            {
                if (formatMilliseconds)
                {
                    formattedTime = string.Format("{0}:{1:D2}:{2:D2}", newTime.Minutes, newTime.Seconds,
                        newTime.Milliseconds);
                }
                else
                {
                    formattedTime = string.Format("{0}:{1:D2}", newTime.Minutes, newTime.Seconds);
                }
            }

            return formattedTime;
        }
        
        public static string ToCamelCaseWithSpace(this string text)
        {
            return Regex.Replace(text, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1").Trim();
        }

        public static string ToPascalCase(this string text)
        {
            return Regex.Replace(text, @"\b\p{Ll}", match => match.Value.ToUpper());
        }
    }
}