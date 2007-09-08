
#region usings

using System;

#endregion

namespace Lephone.Data.Common
{
    public class EmptyCondition : WhereCondition
    {
        public override bool SubClauseNotEmpty
        {
            get { return false; }
        }

        public override string ToSqlText(ref Lephone.Data.SqlEntry.DataParamterCollection dpc, Lephone.Data.Dialect.DbDialect dd)
        {
            return null;
        }
    }
}
