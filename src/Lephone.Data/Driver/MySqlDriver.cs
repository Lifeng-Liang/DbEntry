using System.Data;
using Lephone.Data.Dialect;

namespace Lephone.Data.Driver
{
    internal class MySqlDriver : CommonDbDriver
    {
        public MySqlDriver(DbDialect dialectClass, string connectionString, string dbProviderFactoryName, bool autoCreateTable) 
            : base(dialectClass, connectionString, dbProviderFactoryName, autoCreateTable)
        {
        }

        public override IDbDataParameter GetDbParameter(SqlEntry.DataParameter dp, bool includeSourceColumn)
        {
            var result = base.GetDbParameter(dp, includeSourceColumn);
            if(result.DbType == DbType.Time)
            {
                result.DbType = DbType.DateTime;
            }
            return result;
        }
    }
}
