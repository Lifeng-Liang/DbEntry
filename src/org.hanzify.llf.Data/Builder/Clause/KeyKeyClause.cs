
#region usings

using System;
using org.hanzify.llf.util.Text;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.Data.Builder.Clause
{
    [Serializable]
    public class KeyKeyClause : WhereCondition
    {
		public KeyValue KV;
		public string Comp;

		public KeyKeyClause(string Key1, string Key2)
            : this(Key1, Key2, CompareOpration.Equal)
		{
		}

		public KeyKeyClause(string Key1, string Key2, CompareOpration co)
		{
            KV = new KeyValue(Key1, Key2);
            Comp = StringHelper.EnumToString(co);
        }

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }

        public override string ToSqlText(ref DataParamterCollection dpc, DbDialect dd)
		{
            return string.Format("{0} {2} {1}", dd.QuoteForColumnName(KV.Key), dd.QuoteForColumnName((string)KV.Value), Comp);
		}
    }
}
