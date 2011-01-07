using Lephone.Data.Common;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public abstract class SqlStatementBuilder
	{
        public SqlStatement ToSqlStatement(ObjectInfo info)
        {
            var sql = ToSqlStatement(info.Context.Dialect, info.AllowSqlLog);
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
