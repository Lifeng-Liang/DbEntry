using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Common;

namespace Lephone.Data.Dialect
{
    public class SQLite : DbDialect
    {
        public SQLite()
        {
            TypeNames[DataType.Binary] = "BLOB";
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

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement sql = base.GetNormalSelectSqlStatement(ssb);
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
    }
}
