using System.Data;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;
using Lephone.Data.Common;

namespace Lephone.Data.Dialect
{
	public class SqlServer2000 : DbDialect
	{
		public SqlServer2000()
		{
            TypeNames[DataType.Boolean] = "bit";
            TypeNames[DataType.Date]    = "smalldatetime";
        }

        public override string DbNowString
        {
            get { return "getdate()"; }
        }

        public override DbDriver CreateDbDriver(string ConnectionString, string DbProviderFactoryName)
        {
            return new SqlServerDriver(this, ConnectionString, DbProviderFactoryName);
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new[] { null, null, null, "BASE TABLE" }, null, null, null);
        }

        public override bool SupportsRangeStartIndex
        {
            get { return false; }
        }

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            DataParamterCollection dpc = new DataParamterCollection();
            string SqlString = string.Format("Select Top {4} {0} From {1}{2}{3}",
                ssb.GetColumns(this),
                ssb.From.ToSqlText(dpc, this),
                ssb.Where.ToSqlText(dpc, this),
                (ssb.Order == null || ssb.Keys.Count == 0) ? "" : ssb.Order.ToSqlText(dpc, this),
                ssb.Range.EndIndex
                );
            return new TimeConsumingSqlStatement(CommandType.Text, SqlString, dpc);
        }
        
		public override string IdentitySelectString
		{
			get { return "select SCOPE_IDENTITY();\n"; }
		}

		public override string IdentityColumnString
		{
            get { return "IDENTITY NOT FOR REPLICATION NOT NULL"; }
		}

		public override char CloseQuote
		{
			get { return ']'; }
		}

		public override char OpenQuote
		{
			get { return '['; }
		}

		protected override string QuoteSingle( string name )
		{
			return OpenQuote + name.Replace( CloseQuote.ToString(), new string( CloseQuote, 2 ) ) + CloseQuote;
		}
	}
}
