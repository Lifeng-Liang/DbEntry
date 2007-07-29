
#region usings

using System;
using System.Data;
using System.Data.Common;
using org.hanzify.llf.util;
using org.hanzify.llf.MockSql.Recorder;

#endregion

namespace org.hanzify.llf.MockSql
{
    public class RecorderConnection : DbConnection
    {
        private string _ConnectionString;
        private bool _IsOpened = false;
        internal IRecorder Recorder;

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new RecorderDbTransaction(this, isolationLevel);
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw RecorderFactory.NotImplemented;
        }

        public override void Close()
        {
            _IsOpened = false;
        }

        public override string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
                Recorder = (IRecorder)ClassHelper.CreateInstance(_ConnectionString);
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
            if (!_IsOpened)
            {
                _IsOpened = true;
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

        public override System.Data.ConnectionState State
        {
            get { throw RecorderFactory.NotImplemented; }
        }

        public override System.Data.DataTable GetSchema()
        {
            return new DataTable();
        }

        public override System.Data.DataTable GetSchema(string collectionName)
        {
            return new DataTable();
        }

        public override System.Data.DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return new DataTable();
        }
    }
}
