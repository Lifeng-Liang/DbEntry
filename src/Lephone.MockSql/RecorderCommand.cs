using System.Data.Common;

namespace Lephone.MockSql
{
    public class RecorderCommand : DbCommand
    {
        private string _CommandText;
        private int _CommandTimeout;
        private System.Data.CommandType _CommandType;
        private RecorderConnection _DbConnection;
        private RecorderDbTransaction _DbTransaction;
        private readonly DbParameterCollection _DbParameterCollection = new RecorderParameterCollection();

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
            Record();
            return new RecorderReader();
        }

        public override int ExecuteNonQuery()
        {
            Record();
            return 1;
        }

        public override object ExecuteScalar()
        {
            Record();
            return serial++;
        }

        private void Record()
        {
            if (_DbTransaction == null)
            {
                _DbConnection.Recorder.Write(this.ToString());
            }
            else
            {
                _DbTransaction.sqls.Add(this.ToString());
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

        public override string ToString()
        {
            string s = string.Format("{0}<{1}><{2}>({3})",
                this.CommandText,
                this.CommandType,
                this.CommandTimeout,
                this.Parameters);
            return s;
        }
    }
}
