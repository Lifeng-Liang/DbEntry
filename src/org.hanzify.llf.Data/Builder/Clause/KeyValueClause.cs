
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
	public class KeyValueClause : WhereCondition
	{
		public KeyValue KV;
		public string Comp;

		public KeyValueClause(string Key, object Value)
			: this(Key, Value, CompareOpration.Equal)
		{
		}

		public KeyValueClause(string Key, object Value, CompareOpration co)
			: this(new KeyValue(Key, Value), co)
		{
		}

		public KeyValueClause(KeyValue kv)
			: this(kv, CompareOpration.Equal)
		{
		}

		public KeyValueClause(KeyValue kv, CompareOpration co)
		{
			Comp = StringHelper.EnumToString(co);
			this.KV = kv;
		}

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }
        
        public override string ToSqlText(ref DataParamterCollection dpc, DbDialect dd)
		{
            string dpStr;
            if (DataSetting.UsingParamter)
            {
                dpStr = string.Format(dd.ParamterPrefix + "{0}_{1}", DataParamter.LegalKey(KV.Key), dpc.Count);
                DataParamter dp = new DataParamter(dpStr, KV.NullableValue, KV.ValueType);
                dpc.Add(dp);
            }
            else
            {
                dpStr = DataTypeParser.ParseToString(KV.Value, dd);
            }
			return string.Format("{0} {2} {1}", dd.QuoteForColumnName(KV.Key), dpStr, Comp);
		}

		/*
		public override string ToString()
		{
			return string.Format("[{0}] {2} {1}", KV.Key, KV.ValueString, Comp);
		}
		*/
    }
}
