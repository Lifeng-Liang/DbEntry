using System.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder.Clause
{
    public class InClause : Condition
    {
        private readonly string _column;
        private readonly object[] _args;
        private readonly bool _notIn;

        public InClause(string column, object[] args, bool notIn = false)
        {
            _column = column;
            _args = args;
            _notIn = notIn;
        }

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }

        public override string ToSqlText(DataParameterCollection dpc, DbDialect dd)
        {
            var sb = new StringBuilder();
            sb.Append(dd.QuoteForColumnName(_column));
            if (_notIn)
            {
                sb.Append(" NOT");
            }
            sb.Append(" IN (");
            if(_args.Length == 1 && _args[0].GetType() == typeof(SqlStatement))
            {
                sb.Append(((SqlStatement)_args[0]).SqlCommandText);
            }
            else if (_args.Length == 1 && _args[0].GetType() == typeof(SelectStatementBuilder))
            {
                sb.Append(((SelectStatementBuilder)_args[0]).ToSqlText(dpc, dd));
            }
            else
            {
                foreach (var o in _args)
                {
                    var v = GetValueString(dpc, dd, new KeyValue("in", o));
                    sb.Append(v);
                    sb.Append(",");
                }
                if (_args.Length > 0)
                {
                    sb.Length--;
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
