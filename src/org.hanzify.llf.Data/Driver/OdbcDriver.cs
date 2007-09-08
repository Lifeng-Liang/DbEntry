
#region usings

using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

#endregion

namespace Lephone.Data.Driver
{
    internal class OdbcDriver : DbDriver
	{
        public OdbcDriver(Dialect.DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName)
            : base(DialectClass, ConnectionString, DbProviderFactoryName) { }

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
