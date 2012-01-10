using System.Data;
using System.Data.Common;

namespace Leafing.Data.Driver
{
    public class DbFactory
    {
        private readonly DbProviderFactory _innerFactory;

        protected DbFactory() { }

        public DbFactory(DbProviderFactory factory)
        {
            _innerFactory = factory;
        }

        public virtual IDbCommand CreateCommand()
        {
            return _innerFactory.CreateCommand();
        }

        public virtual IDbConnection CreateConnection()
        {
            return _innerFactory.CreateConnection();
        }

        public virtual IDbDataAdapter CreateDataAdapter()
        {
            return _innerFactory.CreateDataAdapter();
        }

        public virtual IDbDataParameter CreateParameter()
        {
            return _innerFactory.CreateParameter();
        }

        public virtual DbCommandBuilder CreateCommandBuilder()
        {
            return _innerFactory.CreateCommandBuilder();
        }
    }
}
