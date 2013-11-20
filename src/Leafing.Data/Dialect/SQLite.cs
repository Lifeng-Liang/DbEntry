using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using Leafing.Data.Builder;
using Leafing.Data.Driver;
using Leafing.Data.SqlEntry;
using Leafing.Data.Common;

namespace Leafing.Data.Dialect
{
    public class SQLite : DbDialect
    {
        protected override string GetBinaryNameWithLength(string baseType, int length)
        {
            if(length == 0)
            {
                return "BLOB";
            }
            return base.GetBinaryNameWithLength(baseType, length);
        }

        public override void InitConnection(DbDriver driver, IDbConnection conn)
        {
            if(DataSettings.UsingForeignKey)
            {
                using(IDbCommand e = driver.GetDbCommand(new SqlStatement("PRAGMA foreign_keys = ON;"), conn))
                {
                    e.ExecuteNonQuery();
                }
            }
        }

        public override string DbNowString
        {
            get { return "DATETIME(CURRENT_TIMESTAMP, 'localtime')"; }
        }

        public override string GetConnectionString(string connectionString)
        {
            string s = ProcessConnectionnString(connectionString);
            if (s[0] == '@')
            {
                return "Cache Size=102400;Synchronous=Off;Data Source=" + s.Substring(1);
            }
            return s;
        }

        public override bool NeedBracketForJoin
        {
            get { return false; }
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new[] { null, null, null, "table" }, null, null, null);
        }

        public override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb, List<string> queryRequiredFields)
        {
            SqlStatement sql = ssb.GetNormalSelectSqlStatement(this, queryRequiredFields);
            sql.SqlCommandText = string.Format("{0} LIMIT {1}, {2}",
                sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return sql;
        }

        public override string IdentityTypeString
        {
            get { return "INTEGER PRIMARY KEY AUTOINCREMENT"; }
        }

        public override bool IdentityIncludePKString
        {
            get { return true; }
        }

        public override char CloseQuote
        {
            get { return ']'; }
        }

        public override char OpenQuote
        {
            get { return '['; }
        }

        public override string IdentityColumnString
        {
            get { return ""; }
        }

        public override string IdentitySelectString
        {
            get { return "SELECT LAST_INSERT_ROWID();\n"; }
        }

        public override void DropColumns(ModelContext ctx, params string[] columns)
        {
            var n = ctx.Info.From.MainTableName;
            var tn = QuoteForTableName(n);
            var cs = ctx.Provider.GetDbColumnInfoList(n);
            foreach(var column in columns)
            {
                var c = cs.First(p => p.ColumnName == column);
                cs.Remove(c);
            }
            var sb = new StringBuilder();
            foreach(var c in cs)
            {
                sb.Append(QuoteForColumnName(c.ColumnName));
                sb.Append(",");
            }
            sb.Length--;
            DbEntry.UsingTransaction(() =>
                {
                    ctx.Provider.ExecuteNonQuery(string.Format("ALTER TABLE {0} RENAME TO __drop__temp__;", tn));
                    ctx.Operator.Create();
                    ctx.Provider.ExecuteNonQuery(string.Format("INSERT INTO {0} SELECT {1} FROM __drop__temp__;", tn, sb));
                    ctx.Provider.ExecuteNonQuery("DROP TABLE __drop__temp__;");
                });
        }
    }
}
