
#region usings

using System;

#endregion

namespace org.hanzify.llf.Data.Builder.Clause
{
    public class ConstCondition : WhereCondition
    {
        private string Condition;

        internal ConstCondition(string Condition)
        {
            this.Condition = Condition;
        }

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }

        public override string ToSqlText(ref org.hanzify.llf.Data.SqlEntry.DataParamterCollection dpc, org.hanzify.llf.Data.Dialect.DbDialect dd)
        {
            return Condition;
        }
    }
}
