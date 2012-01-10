using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder
{
	public interface IClause
	{
		string ToSqlText(DataParameterCollection dpc, DbDialect dd);
	}
}
