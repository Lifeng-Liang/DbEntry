
#region usings

using System;
using System.Data;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.Builder.Clause;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder
{
	public class DeleteStatementBuilder : ISqlStatementBuilder, ISqlWhere
	{
		private const string StatementTemplate = "Delete From {0}{1};\n";
		private string TableName;
        private WhereClause _WhereOptions = new WhereClause();

		public DeleteStatementBuilder(string TableName)
		{
			this.TableName = TableName;
		}

		public SqlStatement ToSqlStatement(DbDialect dd)
		{
			DataParamterCollection dpc = new DataParamterCollection();
			string SqlString = string.Format(StatementTemplate, dd.QuoteForTableName(TableName), Where.ToSqlText(ref dpc, dd));
			SqlStatement Sql = new SqlStatement(CommandType.Text, SqlString, dpc);
			return Sql;
		}

		public WhereClause Where
		{
			get { return _WhereOptions; }
		}
	}
}
