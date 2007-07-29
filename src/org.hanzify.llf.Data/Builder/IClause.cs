
#region usings

using System;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder
{
	public interface IClause
	{
		string ToSqlText(ref DataParamterCollection dpc, DbDialect dd);
	}
}
