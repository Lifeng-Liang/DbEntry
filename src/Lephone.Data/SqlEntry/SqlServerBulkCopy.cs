using System;
using System.Data;
using System.Data.SqlClient;

namespace Lephone.Data.SqlEntry
{
    public class SqlServerBulkCopy : IDbBulkCopy, IDisposable
    {
        #region properties

        public int BatchSize
        {
            get
            {
                return BulkCopy.BatchSize;
            }
            set
            {
                BulkCopy.BatchSize = value;
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
                BulkCopy.BulkCopyTimeout = value;
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
                BulkCopy.DestinationTableName = value;
            }
        }

        public int NotifyAfter
        {
            get
            {
                return BulkCopy.NotifyAfter;
            }
            set
            {
                BulkCopy.NotifyAfter = value;
            }
        }

        #endregion

        public event SqlRowsCopiedEventHandler SqlRowsCopied;

        private readonly SqlBulkCopy BulkCopy;

        public SqlServerBulkCopy(SqlConnection connection)
        {
            BulkCopy = new SqlBulkCopy(connection);
            BulkCopy.SqlRowsCopied += BulkCopy_SqlRowsCopied;
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
            BulkCopy.Close();
        }

        public void WriteToServer(DataRow[] rows)
        {
            BulkCopy.WriteToServer(rows);
        }

        public void WriteToServer(DataTable table)
        {
            BulkCopy.WriteToServer(table);
        }

        public void WriteToServer(IDataReader reader)
        {
            BulkCopy.WriteToServer(reader);
        }

        public void WriteToServer(DataTable table, DataRowState rowState)
        {
            BulkCopy.WriteToServer(table, rowState);
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)BulkCopy).Dispose();
        }
    }
}
