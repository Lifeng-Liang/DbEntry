using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Lephone.MockSql
{
    public class RecorderDbTransaction : DbTransaction
    {
        private readonly RecorderConnection conn;
        private readonly IsolationLevel level;
        internal List<string> sqls = new List<string>();

        public RecorderDbTransaction(DbConnection c, IsolationLevel l)
        {
            this.conn = (RecorderConnection)c;
            this.level = l;
        }

        public override void Commit()
        {
            foreach (string s in sqls)
            {
                conn.Recorder.Write(s);
            }
        }

        protected override DbConnection DbConnection
        {
            get { return conn; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return level; }
        }

        public override void Rollback()
        {
            sqls.Clear();
        }
    }
}
