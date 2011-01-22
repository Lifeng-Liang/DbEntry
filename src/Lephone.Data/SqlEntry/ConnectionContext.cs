using System;
using System.Data;
using Lephone.Data.Driver;
using Lephone.Core;

namespace Lephone.Data.SqlEntry
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

        public object Jar;

        private IDbConnection _connection;

        public IDbConnection GetConnection(DbDriver driver)
        {
            if (_state == ConnectionContextState.NoConnection)
            {
                _connection = GetDriver(driver).GetDbConnection();
                _connection.Open();
                _state = ConnectionContextState.ConnectionOpened;
            }
            return _connection;
        }

        private IDbTransaction _transaction;

        private IDbTransaction GetTransaction(DbDriver driver)
        {
            if (_state == ConnectionContextState.ConnectionOpened || _state == ConnectionContextState.TransactionEnded)
            {
                switch (_transactionState)
                {
                    case ConnectionContextTransactionState.UnspecifiedTransaction:
                        _transaction = GetConnection(driver).BeginTransaction();
                        _state = ConnectionContextState.TransactionStarted;
                        break;
                    case ConnectionContextTransactionState.SpecifiedTransaciton:
                        _transaction = GetConnection(driver).BeginTransaction(_isolationLevel);
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
        private DbDriver _driver;

        private DbDriver GetDriver(DbDriver driver)
        {
            return _driver ?? (_driver = driver);
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

        public IDbCommand GetDbCommand(SqlStatement sql, DbDriver driver)
        {
            if(_driver == null)
            {
                _driver = driver;
            }
            var e = _driver.GetDbCommand(sql, GetConnection(driver));
            if(GetTransaction(driver) != null)
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
