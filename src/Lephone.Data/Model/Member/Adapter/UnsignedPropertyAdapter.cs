using System.Reflection;

namespace Lephone.Data.Model.Member.Adapter
{
    internal class UnsignedPropertyAdapter : PropertyAdapter
    {
        public UnsignedPropertyAdapter(PropertyInfo pi)
            : base(pi)
        {
        }

        public override void SetValue(object obj, object value)
        {
            if (Info.PropertyType == typeof(ulong))
            {
                Info.SetValue(obj, (ulong)(long)value, null);
            }
            else if (Info.PropertyType == typeof(uint))
            {
                Info.SetValue(obj, (uint)(int)value, null);
            }
            else if (Info.PropertyType == typeof(ushort))
            {
                Info.SetValue(obj, (ushort)(short)value, null);
            }
        }
    }
}
