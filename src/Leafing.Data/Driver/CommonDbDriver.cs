using System;
using System.Data;
using System.Data.Common;
using Leafing.Core.Setting;

namespace Leafing.Data.Driver {
    internal class CommonDbDriver : DbDriver {
        public CommonDbDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme) {
        }

        protected override DbProviderFactory GetDefaultProviderFactory() {
            throw new NotSupportedException();
        }

        protected override void DeriveParameters(IDbCommand e) {
            var f = ProviderFactory as SmartDbFactory;
            if (f != null) {
                f.DeriveParameters(e);
            } else {
                throw new DataException("DeriveParameters not found.");
            }
        }
    }
}
