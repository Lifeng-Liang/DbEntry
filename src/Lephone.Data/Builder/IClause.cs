using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
	public interface IClause
	{
		string ToSqlText(DataParameterCollection dpc, DbDialect dd);
	}
}
