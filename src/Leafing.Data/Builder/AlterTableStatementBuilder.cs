using System.Collections.Generic;
using System.Text;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder {
    public class AlterTableStatementBuilder : SqlStatementBuilder {
        private readonly FromClause _from;
        public ColumnInfo AddColumn;
        public object DefaultValue;
        public bool DefaultFirst = false;

        public AlterTableStatementBuilder(FromClause from, bool defaultFirst) {
            this._from = from;
            this.DefaultFirst = defaultFirst;
        }

        protected override SqlStatement ToSqlStatement(DbDialect dd, List<string> queryRequiredFields) {
            var sb = new StringBuilder("ALTER TABLE ");
            sb.Append(dd.QuoteForTableName(_from.MainTableName));
            if (AddColumn != null) {
                sb.Append(" ADD ");
                sb.Append(dd.QuoteForColumnName(AddColumn.Key));
                sb.Append(" ");
                sb.Append(dd.GetTypeName(AddColumn));
                if (DefaultValue != null) {
                    if (DefaultFirst) {
                        sb.Append(" DEFAULT(").Append(DefaultValue).Append(")");
                        sb.Append(AddColumn.AllowNull ? " NULL" : " NOT NULL");
                    } else {
                        sb.Append(AddColumn.AllowNull ? " NULL" : " NOT NULL");
                        sb.Append(" DEFAULT(").Append(DefaultValue).Append(")");
                    }
                } else {
                    sb.Append(AddColumn.AllowNull ? " NULL" : " NOT NULL");
                }
                sb.Append(";");
            }
            return new SqlStatement(sb.ToString());
        }
    }
}
