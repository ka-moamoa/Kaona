using System;
using System.Collections.Generic;

namespace AudioTextSynchronizer.Tools.Subtitles
{
    public class SubtitleItem
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public List<string> Lines { get; }
        
        public SubtitleItem()
        {
            Lines = new List<string>();
        }

        public override string ToString()
        {
            var startTs = new TimeSpan(0, 0, 0, 0, StartTime);
            var endTs = new TimeSpan(0, 0, 0, 0, EndTime);
            return $"{startTs.ToString()} --> {endTs.ToString()}: {string.Join(Environment.NewLine, Lines.ToArray())}";
        }
    }
}