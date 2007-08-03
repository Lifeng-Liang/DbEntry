
using System;
using System.Data;
using System.Data.SqlClient;

namespace org.hanzify.llf.Data.SqlEntry
{
    public interface IDbBulkCopy
    {
        event SqlRowsCopiedEventHandler SqlRowsCopied;

        int BatchSize { get; set; }
        int BulkCopyTimeout { get; set; }
        string DestinationTableName { get; set; }
        int NotifyAfter { get; set; }

        void Close();
        void WriteToServer(DataRow[] rows);
        void WriteToServer(DataTable table);
        void WriteToServer(IDataReader reader);
        void WriteToServer(DataTable table, DataRowState rowState);
    }
}
