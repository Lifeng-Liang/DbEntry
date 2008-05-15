using System;
using System.Data;
using System.Data.SqlClient;
using Lephone.Data.Builder;
using Lephone.Util;

namespace Lephone.Data.SqlEntry
{
    public class CommonBulkCopy : IDbBulkCopy
    {
        #region properties

        private int _BatchSize;

        public int BatchSize
        {
            get { return _BatchSize; }
            set
            {
                if (value < 0) { throw new DataException("Argument out of scope."); }
                _BatchSize = value;
            }
        }

        private int _BulkCopyTimeout = 30;

        public int BulkCopyTimeout
        {
            get { return _BulkCopyTimeout; }
            set
            {
                if (value < 0) { throw new DataException("Argument out of scope."); }
                _BulkCopyTimeout = value;
            }
        }

        private string _DestinationTableName;

        public string DestinationTableName
        {
            get { return _DestinationTableName; }
            set { _DestinationTableName = value; }
        }

        private int _NotifyAfter;

        public int NotifyAfter
        {
            get { return _NotifyAfter; }
            set
            {
                if (value < 0) { throw new DataException("Argument out of scope."); }
                _NotifyAfter = value;
            }
        }

        #endregion

        public event SqlRowsCopiedEventHandler SqlRowsCopied;

        private readonly DataProvider Provider;

        public CommonBulkCopy(DataProvider Provider)
        {
            this.Provider = Provider;
        }

        public void Close()
        {
            Provider.ConProvider.Close();
        }

        public void WriteToServer(DataRow[] rows)
        {
            Provider.UsingTransaction(delegate
            {
                foreach (DataRow dr in rows)
                {
                    InsertStatementBuilder sb = new InsertStatementBuilder(this.DestinationTableName);
                    DataColumnCollection dcc = dr.Table.Columns;
                    for (int i = 0; i < dcc.Count; i++)
                    {
                        object o = GetValue(dr[i]);
                        sb.Values.Add(new KeyValue(dcc[i].ColumnName, o));
                    }
                    if (!WriteSingleToServer(sb)) { break; }
                }
            });
        }

        public void WriteToServer(DataTable table)
        {
            Provider.UsingTransaction(delegate
            {
                foreach (DataRow dr in table.Rows)
                {
                    InsertStatementBuilder sb = new InsertStatementBuilder(this.DestinationTableName);
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        object o = GetValue(dr[i]);
                        sb.Values.Add(new KeyValue(table.Columns[i].ColumnName, o));
                    }
                    if (!WriteSingleToServer(sb)) { break; }
                }
            });
        }

        public void WriteToServer(IDataReader reader)
        {
            Provider.UsingTransaction(delegate
            {
                while (reader.Read())
                {
                    InsertStatementBuilder sb = new InsertStatementBuilder(this.DestinationTableName);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object o = GetValue(reader[i]);
                        sb.Values.Add(new KeyValue(reader.GetName(i), o));
                    }
                    if (!WriteSingleToServer(sb)) { break; }
                }
            });
        }

        public void WriteToServer(DataTable table, DataRowState rowState)
        {
            Provider.UsingTransaction(delegate
            {
                foreach (DataRow dr in table.Rows)
                {
                    if (dr.RowState == rowState)
                    {
                        InsertStatementBuilder sb = new InsertStatementBuilder(this.DestinationTableName);
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            object o = GetValue(dr[i]);
                            sb.Values.Add(new KeyValue(table.Columns[i].ColumnName, o));
                        }
                        if (!WriteSingleToServer(sb)) { break; }
                    }
                }
            });
        }

        private object GetValue(object o)
        {
            return (o == DBNull.Value) ? null : o;
        }

        private long Count;

        private bool WriteSingleToServer(InsertStatementBuilder sb)
        {
            SqlStatement Sql = sb.ToSqlStatement(Provider.Dialect);
            Sql.SqlTimeOut = _BulkCopyTimeout;
            Provider.ExecuteNonQuery(Sql);
            Count++;
            if (_BatchSize > 0 && (Count % _BatchSize) == 0)
            {
                ConnectionContext cc = Scope<ConnectionContext>.Current;
                cc.Commit();
                cc.BeginTransaction();
            }
            if (SqlRowsCopied != null && _NotifyAfter > 0 && ((Count % _NotifyAfter) == 0))
            {
                SqlRowsCopiedEventArgs e = new SqlRowsCopiedEventArgs(Count);
                SqlRowsCopied(this, e);
                if (e.Abort) { return false; }
            }
            return true;
        }
    }
}
