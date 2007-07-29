
#region usings

using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

using org.hanzify.llf.util;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.Data.Driver
{
    public class DbFactory
    {
        private DbProviderFactory InnerFactory;

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
    }
}
