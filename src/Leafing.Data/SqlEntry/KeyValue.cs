using System;

namespace Leafing.Data.SqlEntry
{
	[Serializable]
	public class KeyValue
	{
		public string Key;
		public object Value;
		public Type ValueType;

		public object NullableValue
		{
			get
			{
				return Value ?? DBNull.Value;
			}
		}

		protected KeyValue() {}

		protected KeyValue(object Value) : this(null, Value) {}

		public KeyValue(string Key, object Value)
            : this(Key, Value, (Value == null) ? typeof(DBNull) : Value.GetType()) { }

		public KeyValue(string Key, object Value, Type ValueType)
		{
			this.Key = Key;
			this.Value = Value;
			this.ValueType = ValueType;
		}
	}
}
