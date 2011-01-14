using System;
using System.Data;
using Lephone.Data.Driver;
using Lephone.Core;

namespace Lephone.Data.SqlEntry
{
    public enum ConnectionContextState
    {
        NoConnection,
        ConnectionOpen,
        TransactionStart,
        TransactionEnd,
    }

    public enum ConnectionContextTransactionState
    {
        NoTransaction,
        UnspecifiedTransaction,
        SpecifiedTransaciton,
    }

    public class ConnectionContext : IDisposable
    {
        #region properties

        private IDbConnection _connection;

        public IDbConnection Connection
        {
            get
            {
                if (_state == ConnectionContextState.NoConnection)
                {
                    _connection = _driver.GetDbConnection();
                    _connection.Open();
                    _state = ConnectionContextState.ConnectionOpen;
                }
                return _connection;
            }
        }

        private IDbTransaction _transaction;

        public IDbTransaction Transaction
        {
            get
            {
                if (_state == ConnectionContextState.ConnectionOpen || _state == ConnectionContextState.TransactionEnd)
                {
                    switch (_transactionState)
                    {
                        case ConnectionContextTransactionState.UnspecifiedTransaction:
                            _transaction = Connection.BeginTransaction();
                            _state = ConnectionContextState.TransactionStart;
                            break;
                        case ConnectionContextTransactionState.SpecifiedTransaciton:
                            _transaction = Connection.BeginTransaction(_isolationLevel);
                            _state = ConnectionContextState.TransactionStart;
                            break;
                    }
                }
                return _transaction;
            }
        }

        private IsolationLevel _isolationLevel;

        internal IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
        }

        private ConnectionContextState _state;
        private ConnectionContextTransactionState _transactionState;
        private readonly DbDriver _driver;

        #endregion

        public ConnectionContext(DbDriver dd)
        {
            _driver = dd;
        }

        public void BeginTransaction()
        {
            _transactionState = ConnectionContextTransactionState.UnspecifiedTransaction;
        }

        public void BeginTransaction(IsolationLevel il)
        {
            _transactionState = ConnectionContextTransactionState.SpecifiedTransaciton;
            _isolationLevel = il;
        }

        public IDbCommand GetDbCommand(SqlStatement sql)
        {
            var e = _driver.GetDbCommand(sql, Connection);
            if(Transaction != null)
            {
                e.Transaction = _transaction;
            }
            return e;
        }

        public void Commit()
        {
            if (_state == ConnectionContextState.TransactionStart)
            {
                _transaction.Commit();
                _state = ConnectionContextState.TransactionEnd;
            }
        }

        public void Rollback()
        {
            if (_state == ConnectionContextState.TransactionStart)
            {
                _transaction.Rollback();
                _state = ConnectionContextState.TransactionEnd;
            }
        }

        public void Close()
        {
            if (_state != ConnectionContextState.NoConnection)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                if (_state == ConnectionContextState.TransactionStart)
                {
                    CommonHelper.CatchAll(() => _transaction.Rollback());
                }
                _transaction.Dispose();
            }
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}
