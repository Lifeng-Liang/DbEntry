using System.Data;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public class DeleteStatementBuilder : ISqlStatementBuilder, ISqlWhere
	{
		private const string StatementTemplate = "Delete From {0}{1};\n";
		private readonly string TableName;
        private readonly WhereClause _WhereOptions = new WhereClause();

		public DeleteStatementBuilder(string TableName)
		{
			this.TableName = TableName;
		}

		public SqlStatement ToSqlStatement(DbDialect dd)
		{
			DataParamterCollection dpc = new DataParamterCollection();
			string SqlString = string.Format(StatementTemplate, dd.QuoteForTableName(TableName), Where.ToSqlText(dpc, dd));
			SqlStatement Sql = new SqlStatement(CommandType.Text, SqlString, dpc);
			return Sql;
		}

		public WhereClause Where
		{
			get { return _WhereOptions; }
		}
	}
}
