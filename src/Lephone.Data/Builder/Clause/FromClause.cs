using System;
using System.Collections.Specialized;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

namespace Lephone.Data.Builder.Clause
{
    public class FromClause : IClause
    {
        private readonly string TableNameMain;
        internal readonly JoinClause[] jcs;
        private readonly HybridDictionary FromStrings = new HybridDictionary();

        public FromClause(string tableName)
        {
            TableNameMain = tableName;
        }

        public FromClause(params JoinClause[] jcs)
        {
            this.jcs = jcs;
        }

        //internal FromClause(params string[] linkColumnNames)
        //{
        //    if (linkColumnNames.Length < 2 || linkColumnNames.Length % 2 != 0)
        //    {
        //        throw new ArgumentException("LinkColumnNames.Length not even or less than 2.");
        //    }
        //    jcs = new JoinClause[linkColumnNames.Length / 2];
        //    for (int i = 0; i < linkColumnNames.Length; i+=2)
        //    {
        //        jcs[i / 2] = new JoinClause(linkColumnNames[i], linkColumnNames[i + 1],
        //            CompareOpration.Equal, JoinMode.Inner);
        //    }
        //}

        public string GetMainTableName()
        {
            return TableNameMain;
        }

        public string ToSqlText(DataParameterCollection dpc, Dialect.DbDialect dd)
        {
            if (TableNameMain != null)
            {
                return dd.QuoteForTableName(TableNameMain);
            }

            if (FromStrings.Contains(dd))
            {
                return (string)FromStrings[dd];
            }

            var sd = new StringDictionary();
            string ret = dd.QuoteForTableName(jcs[0].Table1);
            sd.Add(ret, "");
            for (int i = 0; i < jcs.Length; i++)
            {
                if (i != 0 && dd.NeedBracketForJoin)
                {
                    ret = string.Format("({0})", ret);
                }
                string tn = dd.QuoteForTableName(jcs[i].Table2);
                if (sd.ContainsKey(tn)) { tn = dd.QuoteForTableName(jcs[i].Table1); }
                sd.Add(tn, "");
                ret = string.Format("{0} {3} JOIN {1} On {2}",
                                    ret,
                                    tn,
                                    jcs[i].ToSqlText(dpc, dd),
                                    jcs[i].Mode);
            }
            lock (FromStrings.SyncRoot)
            {
                FromStrings[dd] = ret;
            }
            return ret;
        }
    }
}
