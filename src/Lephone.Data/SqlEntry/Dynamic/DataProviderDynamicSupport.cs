using System.Collections.Generic;
using System.Data;
using Lephone.Data.SqlEntry.Dynamic;

namespace Lephone.Data.SqlEntry
{
    public partial class DataProvider
    {
        public DynamicRow ExecuteDynamicRow(string sql, params object[] args)
        {
            return ExecuteDynamicRow(new SqlStatement(sql, args));
        }

        public DynamicRow ExecuteDynamicRow(SqlStatement sql)
        {
            var list = ExecuteDynamicList(sql);
            if (list.Count >= 1)
            {
                return list[0];
            }
            return null;
        }

        public List<DynamicRow> ExecuteDynamicList(string sql, params object[] args)
        {
            return ExecuteDynamicList(new SqlStatement(sql, args));
        }

        public List<DynamicRow> ExecuteDynamicList(SqlStatement sql)
        {
            var list = new List<DynamicRow>();
            ExecuteDataReader(sql, dr =>
            {
                while (dr.Read())
                {
                    list.Add(GetRow(dr));
                }
            });
            return list;
        }

        public DynamicTable ExecuteDynamicTable(string sql, params object[] args)
        {
            return ExecuteDynamicTable(new SqlStatement(sql, args));
        }

        public DynamicTable ExecuteDynamicTable(SqlStatement sql)
        {
            var table = new DynamicTable();
            ExecuteDataReader(sql, dr =>
            {
                while (dr.Read())
                {
                    if (table.NeedInit)
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            table.AddKey(dr.GetName(i), i);
                        }
                        table.NeedInit = false;
                    }
                    var row = table.NewRow();
                    var cols = new object[dr.FieldCount];
                    dr.GetValues(cols);
                    row.AppendMemberRange(cols);
                }
            });
            return table;
        }

        public List<List<DynamicRow>> ExecuteDynamicSet(string sql, params object[] args)
        {
            return ExecuteDynamicSet(new SqlStatement(sql, args));
        }

        public List<List<DynamicRow>> ExecuteDynamicSet(SqlStatement sql)
        {
            var set = new List<List<DynamicRow>>();
            ExecuteDataReader(sql, dr =>
            {
                do
                {
                    var list = new List<DynamicRow>();
                    while (dr.Read())
                    {
                        list.Add(GetRow(dr));
                    }
                    set.Add(list);
                } while (dr.NextResult());
            });
            return set;
        }

        private static DynamicRow GetRow(IDataReader dr)
        {
            var row = new DynamicRow();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                row.SetMember(dr.GetName(i), dr.GetValue(i));
            }
            return row;
        }
    }
}
