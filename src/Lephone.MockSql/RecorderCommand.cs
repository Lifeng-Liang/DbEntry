using System.Data;
using System.Data.Common;
using Lephone.MockSql.Recorder;

namespace Lephone.MockSql
{
    public class RecorderCommand : DbCommand
    {
        private RecorderConnection _dbConnection;
        private RecorderDbTransaction _dbTransaction;
        private readonly DbParameterCollection _dbParameterCollection = new RecorderParameterCollection();

        private static int _serial = 1;

        public override void Cancel()
        {
            throw RecorderFactory.NotImplemented;
        }

        public override string CommandText { get; set; }

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        protected override DbParameter CreateDbParameter()
        {
            throw RecorderFactory.NotImplemented;
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return _dbConnection;
            }
            set
            {
                _dbConnection = (RecorderConnection)value;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _dbParameterCollection; }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return _dbTransaction;
            }
            set
            {
                _dbTransaction = (RecorderDbTransaction)value;
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

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
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
            if(StaticRecorder.CurRow.Count > 0)
            {
                object o = StaticRecorder.CurRow[0].Value;
                StaticRecorder.CurRow.Clear();
                return o;
            }
            return _serial++;
        }

        private void Record()
        {
            if (_dbTransaction == null)
            {
                _dbConnection.Recorder.Write(this.ToString());
            }
            else
            {
                _dbTransaction.Sqls.Add(this.ToString());
            }
        }

        public override void Prepare()
        {
            throw RecorderFactory.NotImplemented;
        }

        public override UpdateRowSource UpdatedRowSource
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
