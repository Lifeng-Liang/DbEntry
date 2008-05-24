using System;
using System.Web;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;
using Lephone.Util;

namespace Lephone.Web
{
    public class TransactionRequestFilter : IHttpModule
    {
        private VBHelper.TransactionHelper trans;

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.EndRequest += context_EndRequest;
            context.Error += context_Error;
        }

        protected virtual void context_BeginRequest(object sender, EventArgs e)
        {
            trans = new VBHelper.TransactionHelper(DbEntry.Context);
        }

        protected virtual void context_Error(object sender, EventArgs e)
        {
            VBHelper.Rollback();
            Dispose();
        }

        protected virtual void context_EndRequest(object sender, EventArgs e)
        {
            VBHelper.Commit();
            Dispose();
        }

        public void Dispose()
        {
            if(trans != null)
            {
                if (Scope<ConnectionContext>.Current != null)
                {
                    Scope<ConnectionContext>.Current.Dispose();
                }
                trans.Dispose();
                trans = null;
            }
        }
    }
}
