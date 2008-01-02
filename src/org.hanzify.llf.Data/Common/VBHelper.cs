
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using Lephone.Data.SqlEntry;
using Lephone.Util;
using System.Data;

#endregion

namespace Lephone.Data.Common
{
    public static class VBHelper
    {
        public class TransactionHelper : IDisposable
        {
            private DbContext dc;
            private ConnectionContext cc = null;
            private Scope<ConnectionContext> sc = null;
            private bool Committed = false;

            public TransactionHelper(DbContext dc, IsolationLevel il)
            {
                if (dc == null)
                {
                    throw new ArgumentNullException();
                }
                this.dc = dc;
                cc = new ConnectionContext(dc.m_Driver);
                sc = new Scope<ConnectionContext>(cc);
                cc.BeginTransaction(il);
                this.dc.OnBeginTransaction();
            }

            public TransactionHelper(ConnectionContext cc)
            {
                if (cc == null)
                {
                    throw new ArgumentNullException();
                }
                this.cc = cc;
            }

            public void Commit()
            {
                cc.Commit();
                Committed = true;
                if (dc != null)
                {
                    dc.OnCommittedTransaction();
                }
            }

            public void Dispose()
            {
                try
                {
                    if (!Committed)
                    {
                        try
                        {
                            cc.Rollback();
                        }
                        finally
                        {
                            if (dc != null)
                            {
                                dc.OnTransactionError();
                            }
                        }
                    }
                }
                catch
                {
                    try
                    {
                        cc.Close();
                    }
                    catch
                    {
                    }
                }
                if (sc != null) { sc.Dispose(); }
                cc.Dispose();
            }
        }

        public class ConnectionHelper : IDisposable
        {
            private ConnectionContext cc = null;
            private Scope<ConnectionContext> sc = null;

            public ConnectionHelper(DbContext dc)
            {
                cc = new ConnectionContext(dc.m_Driver);
                sc = new Scope<ConnectionContext>(cc);
            }

            public void Dispose()
            {
                try
                {
                    cc.Close();
                }
                catch { }
                cc.Dispose();
            }
        }

        public static TransactionHelper UsingTransaction()
        {
            return new TransactionHelper(DbEntry.Context, IsolationLevel.ReadCommitted);
        }

        public static TransactionHelper UsingTransaction(IsolationLevel il)
        {
            return new TransactionHelper(DbEntry.Context, il);
        }

        public static TransactionHelper UsingTransaction(DbContext dc)
        {
            return new TransactionHelper(dc, IsolationLevel.ReadCommitted);
        }

        public static TransactionHelper UsingTransaction(DbContext dc, IsolationLevel il)
        {
            return new TransactionHelper(dc, il);
        }

        public static TransactionHelper UsingExistedTransaction()
        {
            ConnectionContext cc = Scope<ConnectionContext>.Current;
            if (cc != null)
            {
                return new TransactionHelper(cc);
            }
            return UsingTransaction();
        }

        public static void Commit()
        {
            DbEntry.Context.ConProvider.Commit();
        }
    }
}
