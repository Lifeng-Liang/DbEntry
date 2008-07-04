namespace Lephone.Data.Builder.Clause
{
    public class ConstCondition : WhereCondition
    {
        private readonly string Condition;

        internal ConstCondition(string Condition)
        {
            this.Condition = Condition;
        }

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }

        public override string ToSqlText(SqlEntry.DataParamterCollection dpc, Dialect.DbDialect dd)
        {
            return Condition;
        }
    }
}
