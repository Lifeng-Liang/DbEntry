using System.Data;
using System.Data.Common;
using Leafing.Core;
using Leafing.MockSql.Recorder;

namespace Leafing.MockSql
{
    public class RecorderConnection : DbConnection
    {
        private string _connectionString;
        private bool _isOpened;
        internal IRecorder Recorder;

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new RecorderDbTransaction(this, isolationLevel);
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw RecorderFactory.NotImplemented;
        }

        public override void Close()
        {
            _isOpened = false;
        }

        public override string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
                Recorder = (IRecorder)ClassHelper.CreateInstance(_connectionString);
            }
        }

        protected override DbCommand CreateDbCommand()
        {
            throw RecorderFactory.NotImplemented;
        }

        public override string DataSource
        {
            get { throw RecorderFactory.NotImplemented; }
        }

        public override string Database
        {
            get { throw RecorderFactory.NotImplemented; }
        }

        public override void Open()
        {
            if (!_isOpened)
            {
                _isOpened = true;
                StaticRecorder.ConnectionOpendTimes++;
            }
            else
            {
                throw new MockDbException("The connecion already opened!");
            }
        }

        public override string ServerVersion
        {
            get { throw RecorderFactory.NotImplemented; }
        }

        public override ConnectionState State
        {
            get { throw RecorderFactory.NotImplemented; }
        }

        public override DataTable GetSchema()
        {
            return new DataTable();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return new DataTable();
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return new DataTable();
        }
    }
}
