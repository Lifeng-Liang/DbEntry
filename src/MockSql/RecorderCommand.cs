
#region usings

using System;
using System.Data.Common;
using Lephone.Util;

using Lephone.MockSql.Recorder;

#endregion

namespace Lephone.MockSql
{
    public class RecorderCommand : DbCommand
    {
        private string _CommandText;
        private int _CommandTimeout;
        private System.Data.CommandType _CommandType;
        private RecorderConnection _DbConnection;
        private RecorderDbTransaction _DbTransaction = null;
        private DbParameterCollection _DbParameterCollection = new RecorderParameterCollection();

        private static int serial = 1;

        public override void Cancel()
        {
            throw RecorderFactory.NotImplemented;
        }

        public override string CommandText
        {
            get
            {
                return _CommandText;
            }
            set
            {
                _CommandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return _CommandTimeout;
            }
            set
            {
                _CommandTimeout = value;
            }
        }

        public override System.Data.CommandType CommandType
        {
            get
            {
                return _CommandType;
            }
            set
            {
                _CommandType = value;
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            throw RecorderFactory.NotImplemented;
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return _DbConnection;
            }
            set
            {
                _DbConnection = (RecorderConnection)value;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _DbParameterCollection; }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return _DbTransaction;
            }
            set
            {
                _DbTransaction = (RecorderDbTransaction)value;
            }
        }

        public override bool DesignTimeVisible
        {
            get
            {
                throw RecorderFactory.NotImplemented;
            }
            set
            {
                throw RecorderFactory.NotImplemented;
            }
        }

        protected override DbDataReader ExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            Record(this.CommandText);
            return new RecorderReader();
        }

        public override int ExecuteNonQuery()
        {
            Record(this.CommandText);
            return 0;
        }

        public override object ExecuteScalar()
        {
            Record(this.CommandText);
            return serial++;
        }

        private void Record(string s)
        {
            if (_DbTransaction == null)
            {
                _DbConnection.Recorder.Write(this.CommandText);
            }
            else
            {
                _DbTransaction.sqls.Add(s);
            }
        }

        public override void Prepare()
        {
            throw RecorderFactory.NotImplemented;
        }

        public override System.Data.UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw RecorderFactory.NotImplemented;
            }
            set
            {
                throw RecorderFactory.NotImplemented;
            }
        }
    }
}
