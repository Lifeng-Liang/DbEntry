using System.Collections.Generic;
using System.Data;
using Leafing.Data.Dialect;
using Leafing.Data.Builder.Clause;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder
{
	public class InsertStatementBuilder : SqlStatementBuilder, ISqlValues
	{
		private const string StatementTemplate = "INSERT INTO {0} {1};\n";
		internal FromClause From;
		private readonly ValuesClause _valuesOptions = new ValuesClause();

        public InsertStatementBuilder(FromClause from)
		{
            this.From = from;
		}

        protected override SqlStatement ToSqlStatement(DbDialect dd, List<string> queryRequiredFields)
		{
			var dpc = new DataParameterCollection();
            string sqlString = string.Format(StatementTemplate, From.ToSqlText(dpc, dd), _valuesOptions.ToSqlText(dpc, dd));
			var sql = new SqlStatement(CommandType.Text, sqlString, dpc);
			return sql;
		}

		public KeyValueCollection Values
		{
			get { return _valuesOptions; }
		}
	}
}
