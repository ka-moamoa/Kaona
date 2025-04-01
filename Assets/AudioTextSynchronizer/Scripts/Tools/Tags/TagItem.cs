namespace AudioTextSynchronizer.Tools.Tags
{
	public class TagItem
	{
		public readonly string Name;
		public readonly string OpenTag;
		public readonly string CloseTag;
		public readonly int OpenTagBeginPosition;
		public readonly int OpenTagEndPosition;
		public readonly int CloseTagBeginPosition;
		public readonly int CloseTagEndPosition;

		public TagItem(string name, string openTag, string closeTag, int openTagBeginPosition, int openTagEndPosition, int closeTagBeginPosition, int closeTagEndPosition)
		{
			Name = name;
			OpenTag = openTag;
			CloseTag = closeTag;
			OpenTagBeginPosition = openTagBeginPosition;
			OpenTagEndPosition = openTagEndPosition;
			CloseTagBeginPosition = closeTagBeginPosition;
			CloseTagEndPosition = closeTagEndPosition;
		}

		public bool IsInsideTag(int position)
		{
			return position > OpenTagBeginPosition || position < CloseTagEndPosition;
		}

		public bool IsInsideTagCodes(int position)
		{
			return position > OpenTagBeginPosition && position < OpenTagEndPosition ||
			       position > CloseTagBeginPosition && position < CloseTagEndPosition;
		}

		public int GetSafePosition(int position)
		{
			if (position > OpenTagBeginPosition && position < OpenTagEndPosition)
			{
				return OpenTagEndPosition;
			}

			if (position > CloseTagBeginPosition && position < CloseTagEndPosition)
			{
				return CloseTagEndPosition;
			}

			return position;
		}

		public bool IsInsideTagBody(int position)
		{
			return position >= OpenTagEndPosition && position <= CloseTagBeginPosition;
		}
	}
}