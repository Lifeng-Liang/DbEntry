using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace Lephone.Data.Driver
{
    internal class OdbcDriver : DbDriver
	{
        public OdbcDriver(Dialect.DbDialect dialectClass, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
            : base(dialectClass, connectionString, dbProviderFactoryName, autoCreateTable) { }

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
