
#region usings

using System;
using System.Text;
using System.Collections.Specialized;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.Data.Builder.Clause
{
    public class FromClause : IClause
    {
        private string TableNameMain = null;
        private JoinClause[] jcs = null;
        private HybridDictionary FromStrings = new HybridDictionary();

        public FromClause(string TableName)
        {
            TableNameMain = TableName;
        }

        public FromClause(params JoinClause[] jcs)
        {
            this.jcs = jcs;
        }

        internal FromClause(params string[] LinkColumnNames)
        {
            if (LinkColumnNames.Length < 2 || LinkColumnNames.Length % 2 != 0)
            {
                throw new ArgumentException("LinkColumnNames.Length not even or less than 2.");
            }
            jcs = new JoinClause[LinkColumnNames.Length / 2];
            for (int i = 0; i < LinkColumnNames.Length; i+=2)
            {
                jcs[i / 2] = new JoinClause(LinkColumnNames[i], LinkColumnNames[i + 1],
                    CompareOpration.Equal, JoinMode.Inner);
            }
        }

        public string GetMainTableName()
        {
            return TableNameMain;
        }

        public string ToSqlText(ref DataParamterCollection dpc, org.hanzify.llf.Data.Dialect.DbDialect dd)
        {
            if (TableNameMain != null)
            {
                return dd.QuoteForTableName(TableNameMain);
            }

            if (FromStrings.Contains(dd))
            {
                return (string)FromStrings[dd];
            }
            else
            {
                StringDictionary sd = new StringDictionary();
                string ret = dd.QuoteForTableName(jcs[0].Key1);
                sd.Add(ret, "");
                for (int i = 0; i < jcs.Length; i++)
                {
                    if (i != 0 && dd.NeedBracketForJoin)
                    {
                        ret = string.Format("({0})", ret);
                    }
                    string tn = dd.QuoteForTableName(jcs[i].Key2);
                    if (sd.ContainsKey(tn)) { tn = dd.QuoteForTableName(jcs[i].Key1); }
                    sd.Add(tn, "");
                    ret = string.Format("{0} {3} Join {1} On {2}",
                        ret,
                        tn,
                        jcs[i].ToSqlText(ref dpc, dd),
                        jcs[i].mode);
                }
                lock (FromStrings.SyncRoot)
                {
                    FromStrings[dd] = ret;
                }
                return ret;
            }
        }
    }
}
