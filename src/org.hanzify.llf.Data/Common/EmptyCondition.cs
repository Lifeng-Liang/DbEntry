
#region usings

using System;

#endregion

namespace org.hanzify.llf.Data.Common
{
    public class EmptyCondition : WhereCondition
    {
        public override bool SubClauseNotEmpty
        {
            get { return false; }
        }

        public override string ToSqlText(ref org.hanzify.llf.Data.SqlEntry.DataParamterCollection dpc, org.hanzify.llf.Data.Dialect.DbDialect dd)
        {
            return null;
        }
    }
}
