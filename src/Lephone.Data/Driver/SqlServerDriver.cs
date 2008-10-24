using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Lephone.Data.Driver
{
    internal class SqlServerDriver : DbDriver
	{
        public SqlServerDriver(Dialect.DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName, bool AutoCreateTable)
            : base(DialectClass, ConnectionString, DbProviderFactoryName, AutoCreateTable) { }

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
