
#region usings

using System;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder.Clause
{
	[Serializable]
	public class NotClause : WhereCondition
	{
		private WhereCondition _ic;

		public NotClause(WhereCondition ic)
		{
			_ic = ic;
		}

        public override bool SubClauseNotEmpty
        {
            get
            {
                if (_ic != null)
                {
                    return _ic.SubClauseNotEmpty;
                }
                return false;
            }
        }

        public override string ToSqlText(ref DataParamterCollection dpc, DbDialect dd)
		{
            if (_ic.SubClauseNotEmpty)
            {
                return string.Format("( Not ({0}) )", _ic.ToSqlText(ref dpc, dd));
            }
            return "";
		}

		/*
		public override string ToString()
		{
			return string.Format("( Not ({0}) )", _ic);
		}
		*/
	}
}
