using System.Collections.Generic;

namespace Leafing.Data.Builder.Clause
{
    public class ConstCondition : Condition
    {
        private readonly string _condition;

        internal ConstCondition(string condition)
        {
            this._condition = condition;
        }

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }

        public override string ToSqlText(SqlEntry.DataParameterCollection dpc, Dialect.DbDialect dd, List<string> queryRequiredFields)
        {
            return _condition;
        }
    }
}
