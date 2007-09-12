
using System;
using System.Data;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;
using Lephone.Data.Common;

namespace Lephone.Data.Dialect
{
	public class SqlServer2000 : DbDialect
	{
		public SqlServer2000() : base()
        {
            TypeNames[DataType.Boolean] = "bit";
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
            return new DbStructInterface(null, new string[] { null, null, null, "BASE TABLE" }, null, null, null);
        }

        public override bool SupportsRange
        {
            get { return true; }
        }

        public override bool SupportsRangeStartIndex
        {
            get { return false; }
        }

        public override string NewGuidString()
        {
            return "Guid()";
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
        
        public override string NullColumnString
		{
			get { return " null"; }
		}

		public override string GetDropTableString(string tableName)
		{
			string st = "if exists (select * from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table {0}";
			return String.Format( st, tableName );
		}

		public override bool SupportsIdentitySelectInInsert
		{
			get { return true; }
		}

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		public override string IdentitySelectString
		{
			get { return "select SCOPE_IDENTITY();\n"; }
		}

		public override string IdentityColumnString
		{
            get { return "IDENTITY NOT FOR REPLICATION NOT NULL"; }
		}

        public override string PrimaryKeyString
        {
            get { return "PRIMARY KEY"; }
        }

		public override int MaxAnsiStringSize
		{
			get { return 8000; }
		}

		public override int MaxBinaryBlobSize
		{
			get { return 2147483647; }
		}

		public override int MaxBinarySize
		{
			get { return 8000; }
		}

		public override int MaxStringClobSize
		{
			get { return 1073741823; }
		}

		public override int MaxStringSize
		{
			get { return 4000; }
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
