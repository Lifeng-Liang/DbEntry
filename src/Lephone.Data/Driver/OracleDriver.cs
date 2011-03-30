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

        protected override void SetCommandTimeOut(IDbCommand e, int timeOut)
        {
        }

        public override IDbDataParameter GetDbParameter(SqlEntry.DataParameter dp)
        {
            var result = base.GetDbParameter(dp);
            if(result.DbType == DbType.Guid && result.Value != null && result.Value.GetType() == typeof(Guid))
            {
                result.Value = ((Guid)result.Value).ToByteArray();
            }
            return result;
        }
    }
}
