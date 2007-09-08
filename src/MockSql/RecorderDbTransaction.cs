
#region usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

#endregion

namespace Lephone.MockSql
{
    public class RecorderDbTransaction : DbTransaction
    {
        private RecorderConnection c;
        private IsolationLevel l;
        internal List<string> sqls = new List<string>();

        public RecorderDbTransaction(DbConnection c, IsolationLevel l)
        {
            this.c = (RecorderConnection)c;
            this.l = l;
        }

        public override void Commit()
        {
            foreach (string s in sqls)
            {
                c.Recorder.Write(s);
            }
        }

        protected override DbConnection DbConnection
        {
            get { return c; }
        }

        public override System.Data.IsolationLevel IsolationLevel
        {
            get { return l; }
        }

        public override void Rollback()
        {
            sqls.Clear();
        }
    }
}
