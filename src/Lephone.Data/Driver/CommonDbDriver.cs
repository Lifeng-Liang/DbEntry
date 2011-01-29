using System;
using System.Data;
using System.Data.Common;

namespace Lephone.Data.Driver
{
    internal class CommonDbDriver : DbDriver
	{
        public CommonDbDriver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoCreateTable)
        {
        }

        protected override DbProviderFactory GetDefaultProviderFactory()
        {
            throw new NotSupportedException();
        }

		protected override void DeriveParameters(IDbCommand e)
		{
            var f = ProviderFactory as SmartDbFactory;
			if ( f != null )
			{
                f.DeriveParameters(e);
			}
			else
			{
                throw new DataException("DeriveParameters not found.");
            }
        }
	}
}
