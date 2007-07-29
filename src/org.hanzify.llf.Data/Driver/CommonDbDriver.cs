
#region usings

using System;
using System.Data;
using System.Data.Common;

using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data.Driver
{
	internal class CommonDbDriver : DbDriver
	{
        public CommonDbDriver(Dialect.DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName)
            : base(DialectClass, ConnectionString, DbProviderFactoryName)
        {
        }

        protected override DbProviderFactory GetDefaultProviderFactory()
        {
            throw new NotSupportedException();
        }

		protected override void DeriveParameters(IDbCommand e)
		{
            SmartDbFactory f = ProviderFactory as SmartDbFactory;
			if ( f != null )
			{
                f.DeriveParameters(e);
			}
			else
			{
                throw new DbEntryException("DeriveParameters not found.");
            }
        }
	}
}
