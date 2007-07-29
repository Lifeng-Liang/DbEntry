
#region usings

using System;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.Builder.Clause;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder
{
	public class UpdateStatementBuilder : ISqlStatementBuilder, ISqlValues, ISqlWhere
	{
		private const string StatementTemplate = "Update {0} {1} {2};\n";
		private string TableName;
        private WhereClause _WhereOptions = new WhereClause();
		private SetClause _SetOptions = new SetClause();

		public UpdateStatementBuilder(string TableName)
		{
			this.TableName = TableName;
		}

		public SqlStatement ToSqlStatement(DbDialect dd)
		{
			DataParamterCollection dpc = new DataParamterCollection();
			string SqlString = string.Format(StatementTemplate, dd.QuoteForTableName(TableName),
				_SetOptions.ToSqlText(ref dpc, dd),
				Where.ToSqlText(ref dpc, dd));
			SqlStatement Sql = new SqlStatement(SqlString, dpc);
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
