using System;
using System.Collections.Specialized;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder.Clause
{
    public class FromClause : IClause
    {
        public readonly string MainTableName;
        public readonly string MainModelName;
        public readonly Type PartOf;
        public readonly JoinClause[] JoinClauseList;
        private readonly HybridDictionary _fromStrings = new HybridDictionary();

        public FromClause(string tableName)
        {
            MainTableName = tableName;
            MainModelName = tableName;
        }

        public FromClause(string modelName, string tableName)
        {
            MainModelName = modelName;
            MainTableName = tableName;
        }

        public FromClause(Type partOf)
        {
            this.PartOf = partOf;
        }

        public FromClause(params JoinClause[] joinClauseList)
        {
            this.JoinClauseList = joinClauseList;
        }

        //internal FromClause(params string[] linkColumnNames)
        //{
        //    if (linkColumnNames.Length < 2 || linkColumnNames.Length % 2 != 0)
        //    {
        //        throw new ArgumentException("LinkColumnNames.Length not even or less than 2.");
        //    }
        //    joinClauseList = new JoinClause[linkColumnNames.Length / 2];
        //    for (int i = 0; i < linkColumnNames.Length; i+=2)
        //    {
        //        joinClauseList[i / 2] = new JoinClause(linkColumnNames[i], linkColumnNames[i + 1],
        //            CompareOpration.Equal, JoinMode.Inner);
        //    }
        //}

        public string ToSqlText(DataParameterCollection dpc, Dialect.DbDialect dd)
        {
            if (MainTableName != null)
            {
                return dd.QuoteForTableName(MainTableName);
            }

            if (_fromStrings.Contains(dd))
            {
                return (string)_fromStrings[dd];
            }

            string ret;

            if (PartOf != null)
            {
                ret = dd.QuoteForLimitTableName(ModelContext.GetInstance(PartOf).Info.From.MainTableName);
            }
            else
            {
                var sd = new StringDictionary();
                ret = dd.QuoteForTableName(JoinClauseList[0].Table1);
                sd.Add(ret, "");
                for (int i = 0; i < JoinClauseList.Length; i++)
                {
                    if (i != 0 && dd.NeedBracketForJoin)
                    {
                        ret = string.Format("({0})", ret);
                    }
                    string tn = dd.QuoteForTableName(JoinClauseList[i].Table2);
                    if (sd.ContainsKey(tn)) { tn = dd.QuoteForTableName(JoinClauseList[i].Table1); }
                    sd.Add(tn, "");
                    ret = string.Format("{0} {3} JOIN {1} On {2}",
                                        ret,
                                        tn,
                                        JoinClauseList[i].ToSqlText(dpc, dd),
                                        JoinClauseList[i].Mode);
                }
            }
            lock (_fromStrings.SyncRoot)
            {
                _fromStrings[dd] = ret;
            }
            return ret;
        }
    }
}
