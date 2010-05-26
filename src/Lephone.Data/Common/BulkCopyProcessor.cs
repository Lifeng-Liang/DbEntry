using System;
using System.Reflection;
using Lephone.Data.SqlEntry;
using Lephone.Util;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public abstract class BulkCopyProcessor
    {
        protected readonly DbContext Src = DbContext.GetInstance("Source");
        protected readonly DbContext Dest = DbContext.GetInstance("Destination");

        public static void CopyAll(Type t)
        {
            CopyAll(t.Assembly);
        }

        public static void CopyAll(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (t.BaseType == typeof(BulkCopyProcessor))
                {
                    var o = (BulkCopyProcessor)ClassHelper.CreateInstance(t);
                    o.Run();
                }
            }
        }

        private string _destinationTableName;

        protected string DestinationTableName
        {
            get { return _destinationTableName; }
            set
            {
                _destinationTableName = value;
                if (_clearTableFirst)
                {
                    ClearTable(value);
                }
            }
        }

        private readonly int _batchSize;
        private readonly int _notifyAfter;
        private readonly bool _clearTableFirst;
        private int _times = 1;

        protected BulkCopyProcessor()
            : this(100, 100, true)
        {
        }

        protected BulkCopyProcessor(int batchSize, int notifyAfter, bool clearTableFirst)
        {
            this._batchSize = batchSize;
            this._notifyAfter = notifyAfter;
            this._clearTableFirst = clearTableFirst;
        }

        protected void ClearTable(string tableName)
        {
            var sql = new SqlStatement("delete from " + Dest.Dialect.QuoteForTableName(tableName));
            Dest.ExecuteNonQuery(sql);
            Console.WriteLine("{0} cleared!", tableName);
        }

        protected void BulkCopy(string sql, bool identityInsert)
        {
            Src.ExecuteDataReader(new SqlStatement(sql), dr =>
            {
                Dest.NewConnection(delegate
                {
                    IDbBulkCopy c = Dest.GetDbBulkCopy(identityInsert);
                    c.BatchSize = _batchSize;
                    c.DestinationTableName = DestinationTableName;
                    c.NotifyAfter = _notifyAfter;
                    c.SqlRowsCopied += (sender, e) => Console.WriteLine("{0}: {1}", DestinationTableName, e.RowsCopied);
                    c.WriteToServer(dr);
                });
            });
            Console.WriteLine("{0} ({1}) copied!", DestinationTableName, _times++);
        }

        public abstract void Run();
    }

    public abstract class BulkCopyProcessor<T> : BulkCopyProcessor where T : class, IDbObject
    {
        public override void Run()
        {
            DestinationTableName = ObjectInfo.GetInstance(typeof(T)).From.MainTableName;
            Copy();
        }

        protected abstract void Copy();
    }
}
