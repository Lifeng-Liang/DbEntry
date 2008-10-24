using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace Lephone.Data.Driver
{
    internal class OdbcDriver : DbDriver
	{
        public OdbcDriver(Dialect.DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName, bool AutoCreateTable)
            : base(DialectClass, ConnectionString, DbProviderFactoryName, AutoCreateTable) { }

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
