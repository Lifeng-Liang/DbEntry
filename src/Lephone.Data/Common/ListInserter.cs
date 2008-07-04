using System.Collections;

namespace Lephone.Data.Common
{
	public class ListInserter : IProcessor
	{
		private readonly IList list;

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
