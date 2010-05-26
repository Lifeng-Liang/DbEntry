using System;
using Lephone.Util.Text;
using Lephone.Data.Common;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class KeyValueClause : Condition
	{
		protected KeyValue KV;
		protected string Comp;
	    protected ColumnFunction function;

        public KeyValueClause(string Key, object Value, CompareOpration co, ColumnFunction function)
			: this(new KeyValue(Key, Value), co)
		{
            this.function = function;
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
        
        public override string ToSqlText(DataParameterCollection dpc, DbDialect dd)
        {
            string dpStr 
                = KV.Value == null 
                ? "NULL" 
                : GetValueString(dpc, dd);
            string dkStr = dd.QuoteForColumnName(KV.Key);
            switch (function)
            {
                case ColumnFunction.ToLower:
                    dkStr = string.Format("LOWER({0})", dkStr);
                    break;
                case ColumnFunction.ToUpper:
                    dkStr = string.Format("UPPER({0})", dkStr);
                    break;
            }
            return string.Format("{0} {2} {1}", dkStr, dpStr, Comp);
        }

	    protected virtual string GetValueString(DataParameterCollection dpc, DbDialect dd)
        {
            string dpStr;
            if (DataSettings.UsingParameter)
            {
                dpStr = string.Format(dd.ParameterPrefix + "{0}_{1}", DataParameter.LegalKey(KV.Key), dpc.Count);
                var dp = new DataParameter(dpStr, KV.NullableValue, KV.ValueType);
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
