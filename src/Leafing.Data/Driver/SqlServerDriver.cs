using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Leafing.Data.Common;

namespace Leafing.Data.Driver
{
    internal class SqlServerDriver : DbDriver
	{
        public SqlServerDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme) { }

        protected override DbProviderFactory GetDefaultProviderFactory()
        {
            return SqlClientFactory.Instance;
        }

		protected override void DeriveParameters(IDbCommand e)
		{
			SqlCommandBuilder.DeriveParameters((SqlCommand)e);
		}
	}
}
