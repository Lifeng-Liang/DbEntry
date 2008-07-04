using System;
using System.Collections;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class KeyValueCollection : CollectionBase
	{
		public KeyValueCollection()
		{
		}

		public void Add(KeyValue kv)
		{
			List.Add( kv );
		}

		public void Add(params KeyValue[] kvs)
		{
			foreach ( KeyValue kv in kvs )
			{
				Add( kv );
			}
		}

		public KeyValue this[int index]
		{
			get { return (KeyValue)List[index]; }
			set { List[index] = value; }
		}
	}
}
