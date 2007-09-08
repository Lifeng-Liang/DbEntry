
#region usings

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

#endregion

namespace Lephone.Data.Driver
{
    internal class SqlServerDriver : DbDriver
	{
        public SqlServerDriver(Dialect.DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName)
            : base(DialectClass, ConnectionString, DbProviderFactoryName) { }

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
