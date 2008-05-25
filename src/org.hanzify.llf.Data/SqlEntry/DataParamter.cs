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
			this.Type = DataTypeParser.Parse(o);
            // TODO: temp solution for time
            if (this.Type == DataType.Date && this.Value != null && this.Value.GetType() != typeof(DBNull))
            {
                this.Value = ((IConvertible)this.Value).ToDateTime(null);
            }
            // TODO: temp solution for time
            if (this.Type == DataType.Time && this.Value != null && this.Value.GetType() != typeof(DBNull))
            {
                this.Value = ((IConvertible)this.Value).ToDateTime(null);
            }
		}

        protected void SetTypeByObject(Type t)
		{
			this.Type = DataTypeParser.Parse(t);
            // TODO: temp solution for time
            if (this.Type == DataType.Date && this.Value != null && this.Value.GetType() != typeof(DBNull))
            {
                this.Value = ((IConvertible)this.Value).ToDateTime(null);
            }
            // TODO: temp solution for time
            if (this.Type == DataType.Time && this.Value != null && this.Value.GetType() != typeof(DBNull))
            {
                this.Value = ((IConvertible)this.Value).ToDateTime(null);
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
			DataParamter dp = (DataParamter)obj;
			bool b = (this.Key == dp.Key)
				&& (this.Value == dp.Value)
				&& (this.Type == dp.Type);
			return b;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
            return string.Format("{0}={1}:{2}", Key, Value == DBNull.Value ? "<NULL>" : Value, Type);
		}
	}
}
