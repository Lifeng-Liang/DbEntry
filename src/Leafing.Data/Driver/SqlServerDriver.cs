using System;
using System.Data;
using System.Data.Common;
using Leafing.Core;
using Leafing.Core.Setting;

namespace Leafing.Data.Driver {
    internal class SqlServerDriver : DbDriver {
        public SqlServerDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme) { }

        protected override DbProviderFactory GetDefaultProviderFactory() {
            var type = Type.GetType("System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient");
            return (DbProviderFactory)ClassHelper.GetValue(type, "Instance");
        }

        protected override void DeriveParameters(IDbCommand e) {
            var type = Type.GetType("System.Data.SqlClient.SqlCommandBuilder, System.Data.SqlClient");
            ClassHelper.CallFunction(type, "DeriveParameters", e);
        }
    }
}
