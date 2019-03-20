using System;

namespace Leafing.Data.SqlEntry {
    [Serializable]
    public class KeyValue {
        public string Key;
        public object Value;
        public Type ValueType;

        public object NullableValue {
            get {
                return Value ?? DBNull.Value;
            }
        }

        protected KeyValue() { }

        protected KeyValue(object value) : this(null, value) { }

        public KeyValue(string key, object value)
            : this(key, value, (value == null) ? typeof(DBNull) : value.GetType()) { }

        public KeyValue(string key, object value, Type valueType) {
            this.Key = key;
            this.Value = value;
            this.ValueType = valueType;
        }
    }
}
