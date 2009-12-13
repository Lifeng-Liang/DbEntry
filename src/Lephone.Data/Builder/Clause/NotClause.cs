using System;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class NotClause : Condition
	{
		private readonly Condition _ic;

		public NotClause(Condition ic)
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

        public override string ToSqlText(DataParameterCollection dpc, DbDialect dd)
		{
            if (_ic.SubClauseNotEmpty)
            {
                return string.Format("( NOT ({0}) )", _ic.ToSqlText(dpc, dd));
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
