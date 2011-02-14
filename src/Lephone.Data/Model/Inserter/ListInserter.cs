using System.Collections;

namespace Lephone.Data.Model.Inserter
{
	public class ListInserter : IProcessor
	{
		private readonly IList _list;

		public ListInserter(IList list)
		{
			this._list = list;
		}

		public bool Process(object obj)
		{
			_list.Add( obj );
			return true;
		}
	}
}
