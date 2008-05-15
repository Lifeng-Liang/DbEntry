using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Lephone.Data.SqlEntry;
using Lephone.Util;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public abstract class BulkCopyProcessor
    {
        protected readonly DbContext Src = new DbContext("Source");
        protected readonly DbContext Dest = new DbContext("Destination");

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
                    BulkCopyProcessor o = (BulkCopyProcessor)ClassHelper.CreateInstance(t);
                    o.Run();
                }
            }
        }

        private string _DestinationTableName;

        protected string DestinationTableName
        {
            get { return _DestinationTableName; }
            set
            {
                _DestinationTableName = value;
                if (_ClearTableFirst)
                {
                    ClearTable(value);
                }
            }
        }

        private int _BatchSize;
        private int _NotifyAfter;
        private bool _ClearTableFirst;
        private int _Times = 1;

        public BulkCopyProcessor()
            : this(100, 100, true)
        {
        }

        public BulkCopyProcessor(int BatchSize, int NotifyAfter, bool ClearTableFirst)
        {
            this._BatchSize = BatchSize;
            this._NotifyAfter = NotifyAfter;
            this._ClearTableFirst = ClearTableFirst;
        }

        protected void ClearTable(string TableName)
        {
            SqlStatement sql = new SqlStatement("delete from " + Dest.Dialect.QuoteForTableName(TableName));
            Dest.ExecuteNonQuery(sql);
            Console.WriteLine("{0} cleared!", TableName);
        }

        protected void BulkCopy(string sql)
        {
            Src.ExecuteDataReader(new SqlStatement(sql), delegate(IDataReader dr)
            {
                Dest.UsingConnection(delegate
                {
                    IDbBulkCopy c = Dest.GetDbBulkCopy();
                    c.BatchSize = _BatchSize;
                    c.DestinationTableName = DestinationTableName;
                    c.NotifyAfter = _NotifyAfter;
                    c.SqlRowsCopied += delegate(object sender, SqlRowsCopiedEventArgs e)
                    {
                        Console.WriteLine("{0}: {1}", DestinationTableName, e.RowsCopied);
                    };
                    c.WriteToServer(dr);
                });
            });
            Console.WriteLine("{0} ({1}) copied!", DestinationTableName, _Times++);
        }

        public abstract void Run();
    }

    public abstract class BulkCopyProcessor<T> : BulkCopyProcessor where T : class, IDbObject
    {
        public override void Run()
        {
            DestinationTableName = ObjectInfo.GetInstance(typeof(T)).From.GetMainTableName();
            Copy();
        }

        protected abstract void Copy();
    }
}
