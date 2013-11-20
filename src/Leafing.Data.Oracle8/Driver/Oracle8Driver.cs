using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using Leafing.Data.Common;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Driver
{
    internal class Oracle8Driver : DbDriver
    {
        public Oracle8Driver(Dialect.DbDialect dialectClass, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
            : base(dialectClass, name, connectionString, dbProviderFactoryName, autoScheme)
		{
		}

        protected override void SetCommandTimeOut(IDbCommand e, int timeOut)
        {
        }

        protected override DbProviderFactory GetDefaultProviderFactory()
        {
            return  OracleClientFactory.Instance;
        }

        protected override void DeriveParameters(IDbCommand e)
        {
            OracleCommandBuilder.DeriveParameters((OracleCommand)e);
        }

        public override IDbDataParameter GetDbParameter(DataParameter dp)
		{
            var odp = (OracleParameter)base.GetDbParameter(dp);
            if (dp.Type == DataType.String)
            {
                odp.OracleType = OracleType.VarChar;
            }
            else
            {
                odp.DbType = (DbType)dp.Type;
            }
			return odp;
		}
    }
}
