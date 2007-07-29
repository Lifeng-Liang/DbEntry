
#region usings

using System;
using System.Text;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder.Clause
{
	[Serializable]
	public class WhereClause : IClause
	{
		private WhereCondition _ic;

		public WhereClause()
		{
		}

		public WhereClause(WhereCondition ic)
		{
			_ic = ic;
		}

		public WhereCondition Conditions
		{
			set { _ic = value; }
			get { return _ic; }
		}

		public string ToSqlText(ref DataParamterCollection dpc, DbDialect dd)
		{
			if ( _ic != null )
			{
				string s = _ic.ToSqlText(ref dpc, dd);
                if (s != null)
                {
                    return (s.Length > 0) ? " Where " + s : "";
                }
			}
			return "";
		}

		public static implicit operator WhereClause (WhereCondition iwc)
		{
			return new WhereClause(iwc);
		}
	}
}
