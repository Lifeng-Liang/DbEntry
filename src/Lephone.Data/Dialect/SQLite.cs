using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Common;

namespace Lephone.Data.Dialect
{
	public class SQLite : DbDialect
	{
		public SQLite() {}

        public override string DbNowString
        {
            get { return "datetime(current_timestamp, 'localtime')"; }
        }

        public override string GetConnectionString(string ConnectionString)
        {
            string s = ProcessConnectionnString(ConnectionString);
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
            return new DbStructInterface(null, new[]{null, null, null, "table"}, null, null, null);
        }

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement Sql = base.GetNormalSelectSqlStatement(ssb);
            Sql.SqlCommandText = string.Format("{0} Limit {1}, {2}", 
                Sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return Sql;
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
            get { return "SELECT last_insert_rowid();\n"; }
		}
	}
}
