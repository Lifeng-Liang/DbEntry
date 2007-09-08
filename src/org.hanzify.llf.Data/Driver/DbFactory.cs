
#region usings

using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

using Lephone.Util;
using Lephone.Util.Setting;

#endregion

namespace Lephone.Data.Driver
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
