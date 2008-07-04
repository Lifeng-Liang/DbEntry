using System;
using System.Data;
using Lephone.Data.Driver;
using Lephone.Util;

namespace Lephone.Data.SqlEntry
{
    public class ConnectionContext : IDisposable
    {
        private readonly IDbConnection _Connection;

        public IDbConnection Connection
        {
            get { return _Connection; }
        }

        private IDbTransaction _Transaction;

        public IDbTransaction Transaction
        {
            get { return _Transaction; }
        }

        private bool IsProcessed;

        internal IsolationLevel IsolationLevel
        {
            get { return _Transaction.IsolationLevel; }
        }

        public ConnectionContext(DbDriver dd)
        {
            _Connection = dd.GetDbConnection();
            _Connection.Open();
        }

        public void BeginTransaction()
        {
            _Transaction = _Connection.BeginTransaction();
            IsProcessed = false;
        }

        public void BeginTransaction(IsolationLevel il)
        {
            _Transaction = _Connection.BeginTransaction(il);
        }

        public void Commit()
        {
            if (!IsProcessed)
            {
                _Transaction.Commit();
                IsProcessed = true;
            }
        }

        public void Rollback()
        {
            if (!IsProcessed)
            {
                _Transaction.Rollback();
                IsProcessed = true;
            }
        }

        public void Close()
        {
            _Connection.Close();
        }

        public void Dispose()
        {
            if (_Transaction != null)
            {
                if (!IsProcessed)
                {
                    CommonHelper.CatchAll(() => _Transaction.Rollback());
                }
                _Transaction.Dispose();
            }
            if (_Connection != null)
            {
                _Connection.Dispose();
            }
        }
    }
}
