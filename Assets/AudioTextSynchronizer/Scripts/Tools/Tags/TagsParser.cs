using System;
using System.Collections.Generic;

namespace AudioTextSynchronizer.Tools.Tags
{
    public class TagsParser
    {
	    private List<TagItem> tags = new List<TagItem>();
	    private TagItem[] tagsArray;
	    
	    public TagItem[] Tags
	    {
		    get
		    {
			    if (tagsArray == null)
			    {
				    tagsArray = tags.ToArray();
				    tags.Clear();
			    }
			    return tagsArray;
		    }
	    }
	    
	    private const string OpenBegin = "<";
	    private const string OpenEnd = ">";
	    private const string NameSeparator = "=";
	    private const string CloseBegin = "</";
	    private const string CloseEnd = ">";

	    public TagsParser(string text)
	    {
		    GetTags(text);
	    }
	    
        private void GetTags(string text)
		{
			var position = 0;
			var openBegin = text.IndexOf(OpenBegin, position, StringComparison.Ordinal);
			while (openBegin != -1)
			{
				openBegin = text.IndexOf(OpenBegin, position, StringComparison.Ordinal);
				if (openBegin != -1)
				{
					position = openBegin;
					var openEndPosition = text.IndexOf(OpenEnd, position + OpenBegin.Length, StringComparison.Ordinal);
					var tagName = text.Substring(position + OpenBegin.Length, openEndPosition + OpenBegin.Length - position).
						Split((NameSeparator + OpenEnd).ToCharArray(),StringSplitOptions.RemoveEmptyEntries)[0];
					var closeTag = CloseBegin + tagName + CloseEnd;
					var closeTagPosition = text.IndexOf(closeTag, openEndPosition, StringComparison.Ordinal);
					if (closeTagPosition != -1)
					{
						var openTag = text.Substring(position, openEndPosition + OpenEnd.Length - position);
						var tag = new TagItem(tagName, openTag, closeTag, 
							position, openEndPosition + OpenEnd.Length,
							closeTagPosition, closeTagPosition + closeTag.Length);
						tags.Add(tag);
						position = closeTagPosition + closeTag.Length;
					}
				}
			}
		}
    }
}