using System;

namespace Leafing.MockSql.Recorder
{
    public class RowInfo
    {
        public string Name;
        public Type Type;
        public object Value;

        public RowInfo(object value)
        {
            Value = value;
            if(value != null)
            {
                Type = value.GetType();
            }
        }

        public RowInfo(string name, object value)
            : this(value)
        {
            Name = name;
        }

        public RowInfo(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
