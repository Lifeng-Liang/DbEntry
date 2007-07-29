
#region usings

using System;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.Dialect
{
	public class MySql : DbDialect
	{
		public MySql() : base() {}

        public override bool SupportsRange
        {
            get { return true; }
        }

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement Sql = base.GetNormalSelectSqlStatement(ssb);
            Sql.SqlCommandText = string.Format("{0} Limit {1}, {2}",
                Sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return Sql;
        }

        public override string UnicodeTypePrefix
        {
            get { return ""; }
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(true, null, new string[] { null, null, null, "BASE TABLE" }, null, null, null);
        }

        public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

        public override bool SupportsIdentitySelectInInsert
        {
            get { return true; }
        }

        public override string IdentitySelectString
		{
            get { return "SELECT LAST_INSERT_ID();\n"; }
		}

		public override string IdentityColumnString
		{
			get { return "AUTO_INCREMENT NOT NULL"; }
		}

        public override string PrimaryKeyString
        {
            get { return "PRIMARY KEY"; }
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

		protected override bool SupportsIfExistsBeforeTableName
		{
			get { return true; }
		}
	}
}
