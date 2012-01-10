using System;
using System.Data;
using System.Data.SqlClient;

namespace Leafing.Data.SqlEntry
{
    public class SqlServerBulkCopy : IDbBulkCopy, IDisposable
    {
        #region properties

        public int BatchSize
        {
            get
            {
                return _bulkCopy.BatchSize;
            }
            set
            {
                _bulkCopy.BatchSize = value;
            }
        }

        public int BulkCopyTimeout
        {
            get
            {
                return BulkCopyTimeout;
            }
            set
            {
                _bulkCopy.BulkCopyTimeout = value;
            }
        }

        public string DestinationTableName
        {
            get
            {
                return DestinationTableName;
            }
            set
            {
                _bulkCopy.DestinationTableName = value;
            }
        }

        public int NotifyAfter
        {
            get
            {
                return _bulkCopy.NotifyAfter;
            }
            set
            {
                _bulkCopy.NotifyAfter = value;
            }
        }

        #endregion

        public event SqlRowsCopiedEventHandler SqlRowsCopied;

        private readonly SqlBulkCopy _bulkCopy;

        public SqlServerBulkCopy(SqlConnection connection)
        {
            _bulkCopy = new SqlBulkCopy(connection);
            _bulkCopy.SqlRowsCopied += BulkCopy_SqlRowsCopied;
        }

        void BulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            if (SqlRowsCopied != null)
            {
                SqlRowsCopied(sender, e);
            }
        }

        public void Close()
        {
            _bulkCopy.Close();
        }

        public void WriteToServer(DataRow[] rows)
        {
            _bulkCopy.WriteToServer(rows);
        }

        public void WriteToServer(DataTable table)
        {
            _bulkCopy.WriteToServer(table);
        }

        public void WriteToServer(IDataReader reader)
        {
            _bulkCopy.WriteToServer(reader);
        }

        public void WriteToServer(DataTable table, DataRowState rowState)
        {
            _bulkCopy.WriteToServer(table, rowState);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)_bulkCopy).Dispose();
        }
    }
}
