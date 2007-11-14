
#region usings

using System;
using Lephone.Util.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Builder.Clause
{
    [Serializable]
    public class KeyKeyClause : KeyValueClause
    {
        public KeyKeyClause(CK k1, CK k2, CompareOpration co)
            : this(k1.ColumnName, object.Equals(k2, null) ? null : k2.ColumnName, co)
        {
        }

		public KeyKeyClause(string Key1, string Key2, CompareOpration co)
            : base(new KeyValue(Key1, Key2), co)
		{
        }

        protected override string GetValueString(DataParamterCollection dpc, DbDialect dd)
        {
            return dd.QuoteForColumnName((string)KV.Value);
        }
    }
}
