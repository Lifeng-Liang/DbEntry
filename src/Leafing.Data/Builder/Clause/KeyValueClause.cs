using System;
using System.Collections.Generic;
using Leafing.Core.Text;
using Leafing.Data.Common;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;
using Leafing.Data.Definition;

namespace Leafing.Data.Builder.Clause
{
	[Serializable]
	public class KeyValueClause : Condition
	{
		protected KeyValue KV;
		protected string Comp;
	    protected ColumnFunction function;

        public KeyValueClause(string key, object value, CompareOpration co, ColumnFunction function)
			: this(new KeyValue(key, value), co)
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

        public override string ToSqlText(DataParameterCollection dpc, DbDialect dd, List<string> queryRequiredFields)
        {
            if (queryRequiredFields != null && dpc.FindQueryRequiedFieldOrId == false)
            {
                if (KV.Key == "Id" || queryRequiredFields.Contains(KV.Key))
                {
                    dpc.FindQueryRequiedFieldOrId = true;
                }
            }
            string dpStr = GetValueString(dpc, dd, KV);
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
    }
}
