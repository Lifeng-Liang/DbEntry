using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Lephone.Data.Driver
{
    internal class SqlServerDriver : DbDriver
	{
        public SqlServerDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoCreateTable) { }

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
