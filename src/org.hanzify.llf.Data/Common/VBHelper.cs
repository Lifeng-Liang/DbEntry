
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data.Common
{
    public static class VBHelper
    {
        public class TransactionHelper : IDisposable
        {
            private ConnectionContext cc = null;
            private Scope<ConnectionContext> sc = null;

            public TransactionHelper(DbContext dc)
            {
                cc = new ConnectionContext(dc.m_Driver);
                sc = new Scope<ConnectionContext>(cc);
                cc.BeginTransaction();
            }

            public void Commit()
            {
                cc.Commit();
            }

            public void Dispose()
            {
                try
                {
                    cc.Rollback();
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
                sc.Dispose();
                cc.Dispose();
            }
        }

        public static TransactionHelper UsingTransaction()
        {
            return new TransactionHelper(DbEntry.Context);
        }

        public static TransactionHelper UsingTransaction(DbContext dc)
        {
            return new TransactionHelper(dc);
        }

        public static void Commit()
        {
            DbEntry.Context.ConProvider.Commit();
        }
    }
}
