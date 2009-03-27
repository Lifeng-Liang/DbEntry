using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Common;

namespace Lephone.Data.Dialect
{
	public class MySql : DbDialect
	{
		public MySql()
		{
            TypeNames[DataType.Guid] = "CHAR(36)";
            TypeNames[DataType.Binary] = "BLOB";
        }

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement Sql = base.GetNormalSelectSqlStatement(ssb);
            Sql.SqlCommandText = string.Format("{0} LIMIT {1}, {2}",
                Sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return Sql;
        }

        public override string GetUnicodeTypeString(string AsciiTypeString)
        {
            return AsciiTypeString;
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(true, null, new[] { null, null, null, "BASE TABLE" }, null, null, null);
        }

        public override string IdentitySelectString
		{
            get { return "SELECT LAST_INSERT_ID();\n"; }
		}

		public override string IdentityColumnString
		{
			get { return "AUTO_INCREMENT NOT NULL"; }
		}

        public override char CloseQuote
		{
			get { return '`'; }
		}

		public override char OpenQuote
		{
			get { return '`'; }
		}

        public override char ParamterPrefix
        {
            get { return '?'; }
        }
	}
}
