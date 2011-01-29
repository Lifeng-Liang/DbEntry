using System;
using System.Data;
using Lephone.Data.Dialect;

namespace Lephone.Data.Driver
{
    internal class OracleDriver : CommonDbDriver
    {
        public OracleDriver(DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoCreateTable)
        {
        }

        public override IDbDataParameter GetDbParameter(SqlEntry.DataParameter dp, bool includeSourceColumn)
        {
            var result = base.GetDbParameter(dp, includeSourceColumn);
            if(result.DbType == DbType.Guid && result.Value != null && result.Value.GetType() == typeof(Guid))
            {
                result.Value = ((Guid)result.Value).ToByteArray();
            }
            return result;
        }
    }
}
