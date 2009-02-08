using System;
using System.Data;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class DataParamter : KeyValue
	{
		public DataType Type;
        public ParameterDirection Direction = ParameterDirection.Input;

        internal static string LegalKey(string Key)
        {
            if (Key != null)
            {
                return Key.Replace('.', '_').Replace(' ', '_');
            }
            return null;
        }

		public DataParamter(object Value) : this(null, Value)
		{
			SetTypeByObject(Value);
		}

		public DataParamter(string Key, object Value) : base(LegalKey(Key), Value)
		{
			SetTypeByObject(Value);
		}

        public DataParamter(string Key, object Value, Type ValueType)
            : base(LegalKey(Key), Value, ValueType)
		{
            SetTypeByObject(ValueType);
		}

        public DataParamter(string Key, object Value, Type ValueType, ParameterDirection Direction)
            : base(LegalKey(Key), Value, ValueType)
        {
            SetTypeByObject(ValueType);
            this.Direction = Direction;
        }

        protected void SetTypeByObject(object o)
		{
			Type = DataTypeParser.Parse(o);
            // TODO: temp solution for time
            if (Type == DataType.Date && Value != null && Value.GetType() != typeof(DBNull))
            {
                Value = ((IConvertible)Value).ToDateTime(null);
            }
            // TODO: temp solution for time
            if (Type == DataType.Time && Value != null && Value.GetType() != typeof(DBNull))
            {
                Value = ((IConvertible)Value).ToDateTime(null);
            }
		}

        protected void SetTypeByObject(Type t)
		{
			Type = DataTypeParser.Parse(t);
            // TODO: temp solution for time
            if (Type == DataType.Date && Value != null && Value.GetType() != typeof(DBNull))
            {
                Value = ((IConvertible)Value).ToDateTime(null);
            }
            // TODO: temp solution for time
            if (Type == DataType.Time && Value != null && Value.GetType() != typeof(DBNull))
            {
                Value = ((IConvertible)Value).ToDateTime(null);
            }
        }

        /*
		public DataParamter(string Key, object Value, DataType Type) : this(Key, Value)
		{
			this.Type = Type;
		}
        */

		public override bool Equals(object obj)
		{
			var dp = (DataParamter)obj;
			bool b = (Key == dp.Key)
				&& (Value == dp.Value)
				&& (Type == dp.Type);
			return b;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
            return string.Format("{0}={1}:{2}", Key, Value == DBNull.Value ? "<NULL>" : Value, Type);
		}
	}
}
