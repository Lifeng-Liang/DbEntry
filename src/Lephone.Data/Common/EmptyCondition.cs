namespace Lephone.Data.Common
{
    public class EmptyCondition : WhereCondition
    {
        public override bool SubClauseNotEmpty
        {
            get { return false; }
        }

        public override string ToSqlText(SqlEntry.DataParamterCollection dpc, Dialect.DbDialect dd)
        {
            return null;
        }
    }
}
