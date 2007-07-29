
using System;

namespace org.hanzify.llf.Data.SqlEntry
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
				return ( Value == null ) ? DBNull.Value : Value;
			}
		}

		protected KeyValue() {}

		protected KeyValue(object Value) : this(null, Value) {}

		public KeyValue(string Key, object Value) : this(Key, Value, Value.GetType()) {}

		public KeyValue(string Key, object Value, Type ValueType)
		{
			this.Key = Key;
			this.Value = Value;
			this.ValueType = ValueType;
            if (ValueType.IsGenericType)
            {
                Common.ObjectInfo oi = Common.DbObjectHelper.GetObjectInfo(ValueType.GetGenericArguments()[0]);
                if (oi.KeyFields != null && oi.KeyFields.Length == 1)
                {
                    this.ValueType = oi.KeyFields[0].FieldType;
                }
            }
		}
	}
}
