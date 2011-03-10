using System;
using System.Data;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class DataParameter : KeyValue
	{
		public DataType Type;
        public ParameterDirection Direction = ParameterDirection.Input;
	    public string SourceColumn;

        internal static string LegalKey(string key)
        {
            if (key != null)
            {
                return key.Replace('.', '_').Replace(' ', '_');
            }
            return null;
        }

		public DataParameter(object value) : this(null, value)
		{
			SetTypeByObject(value);
		}

		public DataParameter(string key, object value) : base(LegalKey(key), value)
		{
			SetTypeByObject(value);
		}

        public DataParameter(string key, object value, Type valueType)
            : base(LegalKey(key), value, valueType)
		{
            SetTypeByObject(valueType);
		}

        public DataParameter(string key, object value, Type valueType, ParameterDirection direction)
            : base(LegalKey(key), value, valueType)
        {
            SetTypeByObject(valueType);
            this.Direction = direction;
        }

        public DataParameter(string key, object value, string sourceColumn)
            : this(key, value)
        {
            this.SourceColumn = sourceColumn;
        }

        public DataParameter(string key, object value, Type valueType, ParameterDirection direction, string sourceColumn)
            : this(key, value, valueType, direction)
        {
            this.SourceColumn = sourceColumn;
        }

        protected void SetTypeByObject(object o)
		{
			Type = DataTypeParser.Parse(o);
            // TODO: temp solution for date
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
		public DataParameter(string Key, object Value, DataType Type) : this(Key, Value)
		{
			this.Type = Type;
		}
        */

		public override bool Equals(object obj)
		{
			var dp = (DataParameter)obj;
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
