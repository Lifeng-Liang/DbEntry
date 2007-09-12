
#region usings

using System;

#endregion

namespace Lephone.Data.Builder.Clause
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

        public override string ToSqlText(Lephone.Data.SqlEntry.DataParamterCollection dpc, Lephone.Data.Dialect.DbDialect dd)
        {
            return Condition;
        }
    }
}
