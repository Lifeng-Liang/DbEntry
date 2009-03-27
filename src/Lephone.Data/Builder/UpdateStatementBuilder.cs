using System.Data;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public class UpdateStatementBuilder : ISqlStatementBuilder, ISqlValues, ISqlWhere
	{
		private const string StatementTemplate = "UPDATE {0} {1} {2};\n";
		private readonly string TableName;
        private readonly WhereClause _WhereOptions = new WhereClause();
		private readonly SetClause _SetOptions = new SetClause();

		public UpdateStatementBuilder(string TableName)
		{
			this.TableName = TableName;
		}

		public SqlStatement ToSqlStatement(DbDialect dd)
		{
			var dpc = new DataParamterCollection();
			string SqlString = string.Format(StatementTemplate, dd.QuoteForTableName(TableName),
				_SetOptions.ToSqlText(dpc, dd),
				Where.ToSqlText(dpc, dd));
			var Sql = new SqlStatement(CommandType.Text, SqlString, dpc);
			return Sql;
		}

		public KeyValueCollection Values
		{
			get { return _SetOptions; }
		}

		public WhereClause Where
		{
			get { return _WhereOptions; }
		}
	}
}
