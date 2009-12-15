using System;

namespace Lephone.Data.Common
{
	[Serializable]
	public class Range
	{
        internal long StartIndex = -1;
        internal long EndIndex = -1;

        internal long Offset = -1;
        internal long Rows = -1;

        public Range(long startIndex, long endIndex)
		{
            if (startIndex <= 0 || endIndex < startIndex)
			{
				throw new ArgumentOutOfRangeException();
			}
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;

            this.Offset = startIndex - 1;
            this.Rows = endIndex - Offset;
		}
	}
}
