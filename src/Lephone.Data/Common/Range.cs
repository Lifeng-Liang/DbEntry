using System;

namespace Lephone.Data.Common
{
	[Serializable]
	public class Range
	{
        internal int StartIndex = -1;
        internal int EndIndex = -1;

        internal int Offset = -1;
        internal int Rows = -1;

        public Range(int StartIndex, int EndIndex)
		{
            if (StartIndex <= 0 || EndIndex < StartIndex)
			{
				throw new ArgumentOutOfRangeException();
			}
            this.StartIndex = StartIndex;
            this.EndIndex = EndIndex;

            this.Offset = StartIndex - 1;
            this.Rows = EndIndex - Offset;
		}
	}
}
