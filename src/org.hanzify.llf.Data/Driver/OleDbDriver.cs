
#region usings

using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

using Lephone.Util;
using Lephone.Data.SqlEntry;

#endregion

namespace Lephone.Data.Driver
{
    internal class OleDbDriver : DbDriver
	{
        public OleDbDriver(Dialect.DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName)
            : base(DialectClass, ConnectionString, DbProviderFactoryName)
		{
		}

        protected override DbProviderFactory GetDefaultProviderFactory()
        {
            return OleDbFactory.Instance;
        }

		protected override void DeriveParameters(IDbCommand e)
		{
			OleDbCommandBuilder.DeriveParameters((OleDbCommand)e);
		}

		protected override IDbDataParameter GetDbParameter(DataParamter dp)
		{
            OleDbParameter odp = (OleDbParameter)base.GetDbParameter(dp);
			// TODO: Is OleDb Bug, Or Access Bug? Or, all Drivers bug£¿
			if ( dp.Type == DataType.Date )
			{
				odp.OleDbType = OleDbType.Date;
			}
			else
			{
				odp.DbType = (DbType)dp.Type;
			}
			return odp;
		}
	}
}
