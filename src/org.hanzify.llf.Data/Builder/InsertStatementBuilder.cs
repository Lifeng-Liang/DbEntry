
#region usings

using System;
using System.Data;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;

#endregion

namespace Lephone.Data.Builder
{
	public class InsertStatementBuilder : ISqlStatementBuilder, ISqlValues
	{
		private const string StatementTemplate = "Insert Into {0} {1};\n";
		internal string TableName;
		private ValuesClause _ValuesOptions = new ValuesClause();

        public InsertStatementBuilder(string TableName)
		{
			this.TableName = TableName;
		}

		public SqlStatement ToSqlStatement(DbDialect dd)
		{
			DataParamterCollection dpc = new DataParamterCollection();
			string SqlString = string.Format(StatementTemplate, dd.QuoteForTableName(TableName), _ValuesOptions.ToSqlText(dpc, dd));
			SqlStatement Sql = new SqlStatement(CommandType.Text, SqlString, dpc);
			return Sql;
		}

		public KeyValueCollection Values
		{
			get { return _ValuesOptions; }
		}
	}
}
