using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Driver
{
    internal class OleDbDriver : DbDriver
	{
        public OleDbDriver(Dialect.DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName, bool AutoCreateTable)
            : base(DialectClass, ConnectionString, DbProviderFactoryName, AutoCreateTable)
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
            if ( dp.Type == DataType.DateTime )
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
