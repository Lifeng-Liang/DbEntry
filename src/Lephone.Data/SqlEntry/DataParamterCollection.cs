using System;
using System.Text;
using System.Collections;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class DataParamterCollection : CollectionBase
	{
        private bool _ParamsNameUserSet;

		internal bool UserSetKey
		{
			get { return _ParamsNameUserSet; }
		}

		public DataParamterCollection() {}

		public DataParamterCollection(params object[] Values)
		{
			Add(Values);
		}

		public DataParamterCollection(params DataParamter[] dps)
		{
			Add(dps);
		}

		public void Add(DataParamter dp)
		{
			bool UserSet = !string.IsNullOrEmpty(dp.Key);
			if ( List.Count == 0 )
			{
				_ParamsNameUserSet = UserSet;
			}
			else
			{
				if ( _ParamsNameUserSet != UserSet )
				{
					throw new ArgumentException("DataParamters's key must all set or all not set.");
				}
			}
			List.Add(dp);
		}

		public void Add(params DataParamter[] dps)
		{
			foreach ( DataParamter dp in dps )
			{
				Add(dp);
			}
		}

		public void Add(params object[] Values)
		{
			foreach ( object o in Values )
			{
				Add( new DataParamter( o ) );
			}
		}

		public void Add(DataParamterCollection dpc)
		{
			foreach ( DataParamter dp in dpc )
			{
				Add(dp);
			}
		}

		public DataParamter this[int index]
		{
			get { return (DataParamter)List[index]; }
			set { List[index] = value; }
		}

		public override bool Equals(object obj)
		{
			DataParamterCollection dpc = (DataParamterCollection)obj;
			for ( int i=0; i<List.Count; i++ )
			{
				if ( !this[i].Equals(dpc[i]) )
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach ( DataParamter dp in List )
			{
				sb.Append(dp.ToString());
				sb.Append(",");
			}
			if ( List.Count > 0 )
			{
				sb.Length--;
			}
			return sb.ToString();
		}
	}
}
