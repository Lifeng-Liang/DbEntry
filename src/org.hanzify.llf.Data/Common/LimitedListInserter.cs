
#region usings

using System;
using System.Collections;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Common
{
    public class LimitedListInserter : IProcessor
    {
		private IList list;
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
            return (Count < DataSetting.MaxRecords);
		}
    }
}
