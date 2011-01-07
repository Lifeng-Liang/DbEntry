using System.Data;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public class UpdateStatementBuilder : SqlStatementBuilder, ISqlValues, ISqlWhere
	{
		private const string StatementTemplate = "UPDATE {0} {1} {2};\n";
		private readonly string _tableName;
        private readonly WhereClause _whereOptions = new WhereClause();
		private readonly SetClause _setOptions = new SetClause();

		public UpdateStatementBuilder(string tableName)
		{
			this._tableName = tableName;
		}

        protected override SqlStatement ToSqlStatement(DbDialect dd)
		{
			var dpc = new DataParameterCollection();
			string sqlString = string.Format(StatementTemplate, dd.QuoteForTableName(_tableName),
				_setOptions.ToSqlText(dpc, dd),
				Where.ToSqlText(dpc, dd));
			var sql = new SqlStatement(CommandType.Text, sqlString, dpc);
			return sql;
		}

		public KeyValueCollection Values
		{
			get { return _setOptions; }
		}

		public WhereClause Where
		{
			get { return _whereOptions; }
		}
	}
}
