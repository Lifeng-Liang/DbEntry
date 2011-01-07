using System.Data;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public class DeleteStatementBuilder : SqlStatementBuilder, ISqlWhere
	{
		private const string StatementTemplate = "DELETE FROM {0}{1};\n";
		private readonly string _tableName;
        private readonly WhereClause _whereOptions = new WhereClause();

		public DeleteStatementBuilder(string tableName)
		{
			this._tableName = tableName;
		}

        protected override SqlStatement ToSqlStatement(DbDialect dd)
		{
			var dpc = new DataParameterCollection();
			string sqlString = string.Format(StatementTemplate, dd.QuoteForTableName(_tableName), Where.ToSqlText(dpc, dd));
			var sql = new SqlStatement(CommandType.Text, sqlString, dpc);
			return sql;
		}

		public WhereClause Where
		{
			get { return _whereOptions; }
		}
	}
}
