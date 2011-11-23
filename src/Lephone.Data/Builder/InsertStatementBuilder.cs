using System.Collections.Generic;
using System.Data;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public class InsertStatementBuilder : SqlStatementBuilder, ISqlValues
	{
		private const string StatementTemplate = "INSERT INTO {0} {1};\n";
		internal string TableName;
		private readonly ValuesClause _valuesOptions = new ValuesClause();

        public InsertStatementBuilder(string tableName)
		{
			this.TableName = tableName;
		}

        protected override SqlStatement ToSqlStatement(DbDialect dd, List<string> queryRequiredFields)
		{
			var dpc = new DataParameterCollection();
			string sqlString = string.Format(StatementTemplate, dd.QuoteForTableName(TableName), _valuesOptions.ToSqlText(dpc, dd));
			var sql = new SqlStatement(CommandType.Text, sqlString, dpc);
			return sql;
		}

		public KeyValueCollection Values
		{
			get { return _valuesOptions; }
		}
	}
}
