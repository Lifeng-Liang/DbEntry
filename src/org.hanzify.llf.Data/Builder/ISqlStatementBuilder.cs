
#region usings

using System;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder
{
	public interface ISqlStatementBuilder
	{
		SqlStatement ToSqlStatement(DbDialect dd);
	}
}
