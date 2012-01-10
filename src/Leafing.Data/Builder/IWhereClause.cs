using System.Collections.Generic;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder
{
    public interface IWhereClause
    {
        string ToSqlText(DataParameterCollection dpc, DbDialect dd, List<string> queryRequiredFields);
    }
}
