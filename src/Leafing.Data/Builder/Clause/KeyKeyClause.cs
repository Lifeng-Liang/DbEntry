using System;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;
using Leafing.Data.Definition;

namespace Leafing.Data.Builder.Clause {
    [Serializable]
    public class KeyKeyClause : KeyValueClause {
        public KeyKeyClause(CK k1, CK k2, CompareOpration co)
            : this(k1.ColumnName, Equals(k2, null) ? null : k2.ColumnName, co) {
        }

        public KeyKeyClause(string key1, string key2, CompareOpration co)
            : base(new KeyValue(key1, key2), co) {
        }

        protected override string GetValueString(DataParameterCollection dpc, DbDialect dd, KeyValue kv) {
            if (kv.Value == null) {
                return "NULL";
            }
            return dd.QuoteForColumnName((string)kv.Value);
        }
    }
}