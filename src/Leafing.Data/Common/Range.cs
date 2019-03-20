using System;

namespace Leafing.Data.Common {
    [Serializable]
    public class Range {
        public long StartIndex = -1;
        public long EndIndex = -1;

        public long Offset = -1;
        public long Rows = -1;

        public Range(long startIndex, long endIndex) {
            if (startIndex <= 0 || endIndex < startIndex) {
                throw new ArgumentOutOfRangeException();
            }
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;

            this.Offset = startIndex - 1;
            this.Rows = endIndex - Offset;
        }
    }
}
