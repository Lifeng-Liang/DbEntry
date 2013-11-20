using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using Leafing.Data.Common;

namespace Leafing.Data.Driver
{
    internal class OdbcDriver : DbDriver
	{
        public OdbcDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme) { }

        protected override DbProviderFactory GetDefaultProviderFactory()
        {
            return OdbcFactory.Instance;
        }

		protected override void DeriveParameters(IDbCommand e)
		{
			OdbcCommandBuilder.DeriveParameters((OdbcCommand)e);
		}
	}
}
