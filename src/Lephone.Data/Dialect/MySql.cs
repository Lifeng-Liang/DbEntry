using Lephone.Data.Builder;
using Lephone.Data.Common;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
	public class MySql : DbDialect
	{
		public MySql()
		{
            TypeNames[DataType.Guid] = "CHAR(36)";
            TypeNames[DataType.Binary] = "BLOB";
            TypeNames[DataType.Double] = "DOUBLE";
            TypeNames[DataType.Single] = "FLOAT";
        }

        public override DbDriver CreateDbDriver(string connectionString, string dbProviderFactoryName, bool autoCreateTable)
        {
            return new MySqlDriver(this, connectionString, dbProviderFactoryName, autoCreateTable);
        }

        public override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement sql = ssb.GetNormalSelectSqlStatement(this);
            sql.SqlCommandText = string.Format("{0} LIMIT {1}, {2}",
                sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return sql;
        }

        public override string GetUnicodeTypeString(string asciiTypeString)
        {
            return asciiTypeString;
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

        public override char ParameterPrefix
        {
            get { return '?'; }
        }
	}
}
