
#region usings

using System;
using Lephone.Util.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

#endregion

namespace Lephone.Data.Builder.Clause
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

        public override string ToSqlText(DataParamterCollection dpc, DbDialect dd)
		{
            return string.Format("{0} {2} {1}", dd.QuoteForColumnName(KV.Key), dd.QuoteForColumnName((string)KV.Value), Comp);
		}
    }
}
