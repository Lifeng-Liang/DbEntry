using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public interface IClause
	{
		string ToSqlText(DataParamterCollection dpc, DbDialect dd);
	}
}
