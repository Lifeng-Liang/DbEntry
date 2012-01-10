using System;
using System.Collections.Generic;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder.Clause
{
	[Serializable]
	public class WhereClause : IWhereClause
	{
		private Condition _ic;

		public WhereClause()
		{
		}

		public WhereClause(Condition ic)
		{
			_ic = ic;
		}

		public Condition Conditions
		{
			set { _ic = value; }
			get { return _ic; }
		}

        public string ToSqlText(DataParameterCollection dpc, DbDialect dd, List<string> queryRequiredFields)
		{
			if ( _ic != null )
			{
				string s = _ic.ToSqlText(dpc, dd, queryRequiredFields);
                if (s != null)
                {
                    if (queryRequiredFields != null && !dpc.FindQueryRequiedFieldOrId)
                    {
                        throw new DataException("The QueryRequired fields not found in query.");
                    }
                    return (s.Length > 0) ? " WHERE " + s : "";
                }
			}
			return "";
		}

		public static implicit operator WhereClause (Condition iwc)
		{
			return new WhereClause(iwc);
		}
	}
}
