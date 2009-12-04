using System;
using System.Text;
using System.Collections;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class DataParameterCollection : CollectionBase
	{
        private bool _paramsNameUserSet;

		internal bool UserSetKey
		{
			get { return _paramsNameUserSet; }
		}

		public DataParameterCollection() {}

		public DataParameterCollection(params object[] values)
		{
			Add(values);
		}

		public DataParameterCollection(params DataParameter[] dps)
		{
			Add(dps);
		}

		public void Add(DataParameter dp)
		{
			bool userSet = !string.IsNullOrEmpty(dp.Key);
			if ( List.Count == 0 )
			{
				_paramsNameUserSet = userSet;
			}
			else
			{
				if ( _paramsNameUserSet != userSet )
				{
					throw new ArgumentException("DataParameters's key must all set or all not set.");
				}
			}
			List.Add(dp);
		}

		public void Add(params DataParameter[] dps)
		{
			foreach ( DataParameter dp in dps )
			{
				Add(dp);
			}
		}

		public void Add(params object[] values)
		{
			foreach ( object o in values )
			{
				Add( new DataParameter( o ) );
			}
		}

		public void Add(DataParameterCollection dpc)
		{
			foreach ( DataParameter dp in dpc )
			{
				Add(dp);
			}
		}

		public DataParameter this[int index]
		{
			get { return (DataParameter)List[index]; }
			set { List[index] = value; }
		}

		public override bool Equals(object obj)
		{
			var dpc = (DataParameterCollection)obj;
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
			var sb = new StringBuilder();
			foreach ( DataParameter dp in List )
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
