using System.Data;
using System.Data.Common;

namespace Lephone.Data.Driver
{
    public class DbFactory
    {
        private readonly DbProviderFactory InnerFactory;

        protected DbFactory() { }

        public DbFactory(DbProviderFactory Factory)
        {
            InnerFactory = Factory;
        }

        public virtual IDbCommand CreateCommand()
        {
            return InnerFactory.CreateCommand();
        }

        public virtual IDbConnection CreateConnection()
        {
            return InnerFactory.CreateConnection();
        }

        public virtual IDbDataAdapter CreateDataAdapter()
        {
            return InnerFactory.CreateDataAdapter();
        }

        public virtual IDbDataParameter CreateParameter()
        {
            return InnerFactory.CreateParameter();
        }

        public virtual DbCommandBuilder CreateCommandBuilder()
        {
            return InnerFactory.CreateCommandBuilder();
        }
    }
}
