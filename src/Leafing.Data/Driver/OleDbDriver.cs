using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Leafing.Data.Common;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Driver
{
    internal class OleDbDriver : DbDriver
	{
        public OleDbDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme)
		{
		}

        protected override DbProviderFactory GetDefaultProviderFactory()
        {
            return OleDbFactory.Instance;
        }

		protected override void DeriveParameters(IDbCommand e)
		{
			OleDbCommandBuilder.DeriveParameters((OleDbCommand)e);
		}

		public override IDbDataParameter GetDbParameter(DataParameter dp)
		{
            var odp = (OleDbParameter)base.GetDbParameter(dp);
			// TODO: Is OleDb Bug, Or Access Bug? Or, all Drivers bug£¿
            if (dp.Type == DataType.DateTime || dp.Type == DataType.Time)
            {
                odp.OleDbType = OleDbType.Date;
            }
            else
            {
                odp.DbType = (DbType)dp.Type;
            }
			return odp;
		}
	}
}
