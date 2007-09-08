
#region usings

using System;
using System.Data.Common;

#endregion

namespace Lephone.MockSql
{
    public class RecorderFactory : DbProviderFactory
    {
        internal static readonly DbException NotImplemented = new MockDbException("The method or operation is not implemented.");

        public override DbCommand CreateCommand()
        {
            return new RecorderCommand();
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new RecorderCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            return new RecorderConnection();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new RecorderDataAdapter();
        }

        public override DbParameter CreateParameter()
        {
            return new RecorderParameter();
        }
    }
}
