using System.Data;
using Leafing.Core.Setting;
using Leafing.Data.Dialect;

namespace Leafing.Data.Driver {
    internal class MySqlDriver : CommonDbDriver {
        public MySqlDriver(DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme) {
        }

        public override IDbDataParameter GetDbParameter(SqlEntry.DataParameter dp) {
            var result = base.GetDbParameter(dp);
            if (result.DbType == DbType.Time) {
                result.DbType = DbType.DateTime;
            }
            return result;
        }
    }
}
