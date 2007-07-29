
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using org.hanzify.llf.Data.Driver;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
{
    public class ConnectionContext : IDisposable
    {
        internal IDbConnection Connection = null;
        internal IDbTransaction Transaction = null;

        private bool IsProcessed = false;

        internal IsolationLevel IsolationLevel
        {
            get { return Transaction.IsolationLevel; }
        }

        public ConnectionContext(DbDriver dd)
        {
            Connection = dd.GetDbConnection();
            Connection.Open();
        }

        public void BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel il)
        {
            Transaction = Connection.BeginTransaction(il);
        }

        public void Commit()
        {
            if (!IsProcessed)
            {
                Transaction.Commit();
                IsProcessed = true;
            }
        }

        public void Rollback()
        {
            if (!IsProcessed)
            {
                Transaction.Rollback();
                IsProcessed = true;
            }
        }

        public void Close()
        {
            Connection.Close();
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                if (!IsProcessed)
                {
                    try
                    {
                        Transaction.Rollback();
                    }
                    catch { }
                }
                Transaction.Dispose();
            }
            if (Connection != null)
            {
                Connection.Dispose();
            }
        }
    }
}
