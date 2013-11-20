using System;
using System.Collections.Generic;
using System.Data;
using Leafing.Core;

namespace Leafing.Data.SqlEntry
{
    public enum ConnectionContextState
    {
        NoConnection,
        ConnectionOpened,
        TransactionStarted,
        TransactionEnded,
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

        public Dictionary<string, object> Jar;

        private IDbConnection _connection;

        public IDbConnection GetConnection(DataProvider provider)
        {
            if (_state == ConnectionContextState.NoConnection)
            {
                CheckContextAreSame(provider);
                _connection = provider.Driver.GetDbConnection();
                _connection.Open();
                provider.Dialect.InitConnection(provider.Driver, _connection);
                _state = ConnectionContextState.ConnectionOpened;
            }
            return _connection;
        }

        private IDbTransaction _transaction;

        private IDbTransaction GetTransaction(DataProvider provider)
        {
            if (_state == ConnectionContextState.ConnectionOpened || _state == ConnectionContextState.TransactionEnded)
            {
                switch (_transactionState)
                {
                    case ConnectionContextTransactionState.UnspecifiedTransaction:
                        _transaction = GetConnection(provider).BeginTransaction();
                        _state = ConnectionContextState.TransactionStarted;
                        break;
                    case ConnectionContextTransactionState.SpecifiedTransaciton:
                        _transaction = GetConnection(provider).BeginTransaction(_isolationLevel);
                        _state = ConnectionContextState.TransactionStarted;
                        break;
                }
            }
            return _transaction;
        }

        private IsolationLevel _isolationLevel;

        internal IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
        }

        private ConnectionContextState _state;
        private ConnectionContextTransactionState _transactionState;
        private string _contextName;

        private void CheckContextAreSame(DataProvider provider)
        {
            if(_contextName == null)
            {
                _contextName = provider.Driver.Name;
            }
            else
            {
                if(_contextName != provider.Driver.Name)
                {
                    throw new DataException("The transaction should use [{0}] but was [{1}]", 
                        _contextName, provider.Driver.Name);
                }
            }
        }

        public bool IsInTransaction
        {
            get { return _state == ConnectionContextState.TransactionStarted; }
        }

        #endregion

        public void BeginTransaction()
        {
            _transactionState = ConnectionContextTransactionState.UnspecifiedTransaction;
        }

        public void BeginTransaction(IsolationLevel il)
        {
            _transactionState = ConnectionContextTransactionState.SpecifiedTransaciton;
            _isolationLevel = il;
        }

        public IDbCommand GetDbCommand(SqlStatement sql, DataProvider provider)
        {
            CheckContextAreSame(provider);
            var e = provider.Driver.GetDbCommand(sql, GetConnection(provider));
            if (GetTransaction(provider) != null)
            {
                e.Transaction = _transaction;
            }
            return e;
        }

        public void Commit()
        {
            if (_state == ConnectionContextState.TransactionStarted)
            {
                _transaction.Commit();
                _state = ConnectionContextState.TransactionEnded;
            }
        }

        public void Rollback()
        {
            if (_state == ConnectionContextState.TransactionStarted)
            {
                _transaction.Rollback();
                _state = ConnectionContextState.TransactionEnded;
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
                if (_state == ConnectionContextState.TransactionStarted)
                {
                    Util.CatchAll(() => _transaction.Rollback());
                }
                else
                {
                    _transaction.Dispose();
                }
            }
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}
