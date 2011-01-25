using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public abstract class SqlStatementBuilder
	{
        public SqlStatement ToSqlStatement(ModelContext ctx)
        {
            var sql = ToSqlStatement(ctx.Provider.Dialect, ctx.Info.AllowSqlLog);
            return sql;
        }

        public SqlStatement ToSqlStatement(DbDialect dd, bool needLog = true)
		{
		    var sql = ToSqlStatement(dd);
		    sql.NeedLog = needLog;
		    return sql;
		}

	    protected abstract SqlStatement ToSqlStatement(DbDialect dd);
	}
}
