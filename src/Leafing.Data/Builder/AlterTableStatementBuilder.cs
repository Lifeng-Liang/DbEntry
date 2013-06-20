using System.Collections.Generic;
using System.Text;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder
{
    public class AlterTableStatementBuilder : SqlStatementBuilder
    {
        private readonly FromClause _from;
        public ColumnInfo AddColumn;
        public string DropColumnName;
        public object DefaultValue;

        public AlterTableStatementBuilder(FromClause from)
        {
            this._from = from;
        }

        protected override SqlStatement ToSqlStatement(DbDialect dd, List<string> queryRequiredFields)
        {
            var sb = new StringBuilder("ALTER TABLE ");
            sb.Append(dd.QuoteForTableName(_from.MainTableName));
            if(AddColumn != null)
            {
                sb.Append(" ADD ");
                sb.Append(dd.QuoteForColumnName(AddColumn.Key));
                sb.Append(" ");
                sb.Append(dd.GetTypeName(AddColumn));
                sb.Append(AddColumn.AllowNull ? " NULL" : " NOT NULL");
                if(DefaultValue != null)
                {
                    sb.Append(" DEFAULT(");
                    sb.Append(DefaultValue);
                    sb.Append(")");
                }
                sb.Append(";");
            }
            else if(!DropColumnName.IsNullOrEmpty())
            {
                sb.Append(" DROP COLUMN ");
                sb.Append(dd.QuoteForColumnName(DropColumnName));
                sb.Append(";");
            }
            return new SqlStatement(sb.ToString());
        }
    }
}
