using System;
using System.Data;
using System.Data.Common;
using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Driver {
    internal class OleDbDriver : DbDriver {
        public OleDbDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme) {
        }

        protected override DbProviderFactory GetDefaultProviderFactory() {
            var type = Type.GetType("System.Data.OleDb.OleDbFactory, System.Data.OleDb");
            return (DbProviderFactory)ClassHelper.GetValue(type, "Instance");
        }

        protected override void DeriveParameters(IDbCommand e) {
            var type = Type.GetType("System.Data.OleDb.OleDbCommandBuilder, System.Data.OleDb");
            ClassHelper.CallFunction(type, "DeriveParameters", e);
        }

        public override IDbDataParameter GetDbParameter(DataParameter dp) {
            var odp = base.GetDbParameter(dp);
            // TODO: Is OleDb Bug, Or Access Bug? Or, all Drivers bug？
            if (dp.Type == DataType.DateTime || dp.Type == DataType.Time) {
                var type = Type.GetType("System.Data.OleDb.OleDbParameter, System.Data.OleDb");
                type.GetProperty("OleDbType").GetSetMethod().Invoke(odp, new object[] { 7 });
            } else {
                odp.DbType = (DbType)dp.Type;
            }
            return odp;
        }
    }
}
