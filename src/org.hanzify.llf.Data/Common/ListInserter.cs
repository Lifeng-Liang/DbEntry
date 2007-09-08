
#region usings

using System;
using System.Collections;

#endregion

namespace Lephone.Data.Common
{
	public class ListInserter : IProcessor
	{
		private IList list;

		public ListInserter(IList list)
		{
			this.list = list;
		}

		public bool Process(object obj)
		{
			list.Add( obj );
			return true;
		}
	}
}
