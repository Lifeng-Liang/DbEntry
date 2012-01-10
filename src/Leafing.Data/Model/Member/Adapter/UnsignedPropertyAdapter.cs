using System.Reflection;

namespace Leafing.Data.Model.Member.Adapter
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
                base.SetValue(obj, (ulong)(long)value);
            }
            else if (Info.PropertyType == typeof(uint))
            {
                base.SetValue(obj, (uint)(int)value);
            }
            else if (Info.PropertyType == typeof(ushort))
            {
                base.SetValue(obj, (ushort)(short)value);
            }
        }
    }
}
