using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Lephone.MockSql
{
    public class RecorderDbTransaction : DbTransaction
    {
        private readonly RecorderConnection _conn;
        private readonly IsolationLevel _level;
        internal List<string> Sqls = new List<string>();

        public RecorderDbTransaction(DbConnection c, IsolationLevel l)
        {
            this._conn = (RecorderConnection)c;
            this._level = l;
        }

        public override void Commit()
        {
            foreach (string s in Sqls)
            {
                _conn.Recorder.Write(s);
            }
        }

        protected override DbConnection DbConnection
        {
            get { return _conn; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return _level; }
        }

        public override void Rollback()
        {
            Sqls.Clear();
        }
    }
}
