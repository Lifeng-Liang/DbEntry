using System;
using System.Data;
using System.Data.SqlClient;
using Lephone.Data.Builder;
using Lephone.Core;

namespace Lephone.Data.SqlEntry
{
    public class CommonBulkCopy : IDbBulkCopy
    {
        #region properties

        private int _batchSize;

        public int BatchSize
        {
            get { return _batchSize; }
            set
            {
                if (value < 0) { throw new DataException("Argument out of scope."); }
                _batchSize = value;
            }
        }

        private int _bulkCopyTimeout = 30;

        public int BulkCopyTimeout
        {
            get { return _bulkCopyTimeout; }
            set
            {
                if (value < 0) { throw new DataException("Argument out of scope."); }
                _bulkCopyTimeout = value;
            }
        }

        public string DestinationTableName { get; set; }

        private int _notifyAfter;

        public int NotifyAfter
        {
            get { return _notifyAfter; }
            set
            {
                if (value < 0) { throw new DataException("Argument out of scope."); }
                _notifyAfter = value;
            }
        }

        #endregion

        public event SqlRowsCopiedEventHandler SqlRowsCopied;

        private readonly DataProvider _provider;

        private readonly bool _identityInsert;

        public CommonBulkCopy(DataProvider provider, bool identityInsert)
        {
            this._provider = provider;
            this._identityInsert = identityInsert;
        }

        public void Close()
        {
            _provider.ConProvider.Close();
        }

        public void WriteToServer(DataRow[] rows)
        {
            ProcessWrite(delegate
            {
                foreach (DataRow dr in rows)
                {
                    var sb = new InsertStatementBuilder(this.DestinationTableName);
                    var dcc = dr.Table.Columns;
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
            ProcessWrite(delegate
            {
                foreach (DataRow dr in table.Rows)
                {
                    var sb = new InsertStatementBuilder(this.DestinationTableName);
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
            ProcessWrite(delegate
            {
                while (reader.Read())
                {
                    var sb = new InsertStatementBuilder(this.DestinationTableName);
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
            ProcessWrite(delegate
            {
                foreach (DataRow dr in table.Rows)
                {
                    if (dr.RowState == rowState)
                    {
                        var sb = new InsertStatementBuilder(this.DestinationTableName);
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

        private void ProcessWrite(CallbackVoidHandler callback)
        {
            _provider.NewTransaction(() =>
            {
                if(_identityInsert)
                {
                    var name = _provider.Dialect.QuoteForTableName(DestinationTableName);
                    _provider.ExecuteNonQuery(string.Format("SET IDENTITY_INSERT {0} ON", name));
                    callback();
                    _provider.ExecuteNonQuery(string.Format("SET IDENTITY_INSERT {0} OFF", name));
                }
                else
                {
                    callback();
                }
            });
        }

        private static object GetValue(object o)
        {
            return (o == DBNull.Value) ? null : o;
        }

        private long _count;

        private bool WriteSingleToServer(InsertStatementBuilder sb)
        {
            SqlStatement sql = sb.ToSqlStatement(_provider.Dialect);
            sql.SqlTimeOut = _bulkCopyTimeout;
            _provider.ExecuteNonQuery(sql);
            _count++;
            if (_batchSize > 0 && (_count % _batchSize) == 0)
            {
                ConnectionContext cc = Scope<ConnectionContext>.Current;
                cc.Commit();
                cc.BeginTransaction();
            }
            if (SqlRowsCopied != null && _notifyAfter > 0 && ((_count % _notifyAfter) == 0))
            {
                var e = new SqlRowsCopiedEventArgs(_count);
                SqlRowsCopied(this, e);
                if (e.Abort) { return false; }
            }
            return true;
        }
    }
}
