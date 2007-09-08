
#region usings

using System;
using System.Data.Common;

#endregion

namespace Lephone.MockSql
{
    public class RecorderCommandBuilder : DbCommandBuilder
    {
        protected override void ApplyParameterInfo(DbParameter parameter, System.Data.DataRow row, System.Data.StatementType statementType, bool whereClause)
        {
            throw RecorderFactory.NotImplemented; ;
        }

        protected override string GetParameterName(string parameterName)
        {
            throw RecorderFactory.NotImplemented; ;
        }

        protected override string GetParameterName(int parameterOrdinal)
        {
            throw RecorderFactory.NotImplemented; ;
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal)
        {
            throw RecorderFactory.NotImplemented; ;
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
            throw RecorderFactory.NotImplemented; ;
        }
    }
}
