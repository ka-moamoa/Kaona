using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AudioTextSynchronizer.Tools.Subtitles
{
    public class SrtParser
    {
        private readonly string[] separators = { "-->" , "- >", "->" };

        public List<SubtitleItem> Parse(Stream stream, Encoding encoding)
        {
            if (!stream.CanRead || !stream.CanSeek)
            {
                var message = $"Stream must be seekable and readable! isSeekable: {stream.CanSeek} - isReadable: {stream.CanSeek}";
                throw new ArgumentException(message);
            }

            stream.Position = 0;
            var reader = new StreamReader(stream, encoding, true);
            var items = new List<SubtitleItem>();
            var srtSubParts = GetSrtSubTitleParts(reader).ToList();
            if (srtSubParts.Any())
            {
                foreach (var srtSubPart in srtSubParts)
                {
                    var lines = srtSubPart.Split(new[] {Environment.NewLine}, StringSplitOptions.None)
                            .Select(s => s.Trim())
                            .Where(l => !string.IsNullOrEmpty(l))
                            .ToList();

                    var item = new SubtitleItem();
                    foreach (var line in lines)
                    {
                        if (item.StartTime == 0 && item.EndTime == 0)
                        {
                            var success = TryParseTimeCodeLine(line, out var startTime, out var endTime);
                            if (success)
                            {
                                item.StartTime = startTime;
                                item.EndTime = endTime;
                            }
                        }
                        else
                        {
                            item.Lines.Add(line);
                        }
                    }

                    if ((item.StartTime != 0 || item.EndTime != 0) && item.Lines.Any())
                    {
                        items.Add(item);
                    }
                }

                if (items.Any())
                {
                    return items;
                }
                throw new ArgumentException("Stream is not in a valid srt format");
            }
            throw new FormatException("Parsing as srt returned no srt part.");
        }
        
        private IEnumerable<string> GetSrtSubTitleParts(TextReader reader)
        {
            string line;
            var sb = new StringBuilder();

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    var res = sb.ToString().TrimEnd();
                    if (!string.IsNullOrEmpty(res))
	                {
		                yield return res;
	                }
                    sb = new StringBuilder();
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

        private bool TryParseTimeCodeLine(string line, out int startTime, out int endTime)
        {
            var parts = line.Split(separators, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                startTime = -1;
                endTime = -1;
                return false;
            }
            startTime = ParseSrtTimeCode(parts[0]);
            endTime = ParseSrtTimeCode(parts[1]);
            return true;
        }

        private static int ParseSrtTimeCode(string s)
        {
            var match = Regex.Match(s, "[0-9]+:[0-9]+:[0-9]+([,\\.][0-9]+)?");
            if (match.Success)
            {
                s = match.Value;
                if (TimeSpan.TryParse(s.Replace(',', '.'), out var result))
                {
                    var nbOfMs = (int)result.TotalMilliseconds;
                    return nbOfMs;
                }
            }
            return -1;
        }
    }
}