using System;
using Lephone.Util.Text;
using Lephone.Data.Common;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class KeyValueClause : WhereCondition
	{
		protected KeyValue KV;
		protected string Comp;
	    protected bool ToLower;

		public KeyValueClause(string Key, object Value, CompareOpration co, bool ToLower)
			: this(new KeyValue(Key, Value), co)
		{
		    this.ToLower = ToLower;
		}

		public KeyValueClause(KeyValue kv, CompareOpration co)
		{
			this.KV = kv;
            if (kv.Value == null)
            {
                if (co == CompareOpration.Equal)
                    co = CompareOpration.Is;
                else if (co == CompareOpration.NotEqual)
                    co = CompareOpration.IsNot;
            }
            Comp = StringHelper.EnumToString(co);
		}

        public override bool SubClauseNotEmpty
        {
            get { return true; }
        }
        
        public override string ToSqlText(DataParamterCollection dpc, DbDialect dd)
        {
            string dpStr 
                = KV.Value == null 
                ? "NULL" 
                : GetValueString(dpc, dd);
            string dkStr = dd.QuoteForColumnName(KV.Key);
            if(ToLower)
            {
                dkStr = string.Format("lower({0})", dkStr);
            }
            return string.Format("{0} {2} {1}", dkStr, dpStr, Comp);
        }

	    protected virtual string GetValueString(DataParamterCollection dpc, DbDialect dd)
        {
            string dpStr;
            if (DataSetting.UsingParamter)
            {
                dpStr = string.Format(dd.ParamterPrefix + "{0}_{1}", DataParamter.LegalKey(KV.Key), dpc.Count);
                var dp = new DataParamter(dpStr, KV.NullableValue, KV.ValueType);
                dpc.Add(dp);
            }
            else
            {
                dpStr = DataTypeParser.ParseToString(KV.Value, dd);
            }
            return dpStr;
        }

		/*
		public override string ToString()
		{
			return string.Format("[{0}] {2} {1}", KV.Key, KV.ValueString, Comp);
		}
		*/
    }
}
