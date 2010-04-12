using System.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder.Clause
{
    public class InClause : Condition
    {
        private readonly string _column;
        private readonly object[] _args;

        public InClause(string column, params object[] args)
        {
            _column = column;
            _args = args;
        }

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }

        public override string ToSqlText(DataParameterCollection dpc, DbDialect dd)
        {
            var sb = new StringBuilder();
            sb.Append(dd.QuoteForColumnName(_column));
            sb.Append(" IN (");
            foreach (var o in _args)
            {
                sb.Append(o);
                sb.Append(",");
            }
            if(_args.Length > 0)
            {
                sb.Length--;
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
