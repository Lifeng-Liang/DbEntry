using System.Collections;

namespace Lephone.Data.Common
{
    public class LimitedListInserter : IProcessor
    {
		private readonly IList list;
        private int Count;

        public LimitedListInserter(IList list)
		{
			this.list = list;
            Count = 0;
		}

		public bool Process(object obj)
		{
			list.Add( obj );
            Count++;
            return (Count < DataSettings.MaxRecords);
		}
    }
}
