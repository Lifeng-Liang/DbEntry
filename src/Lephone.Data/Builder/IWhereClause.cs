using System.Collections.Generic;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder
{
    public interface IWhereClause
    {
        string ToSqlText(DataParameterCollection dpc, DbDialect dd, List<string> queryRequiredFields);
    }
}
