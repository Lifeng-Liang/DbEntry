using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public interface ISqlStatementBuilder
	{
		SqlStatement ToSqlStatement(DbDialect dd);
	}
}
