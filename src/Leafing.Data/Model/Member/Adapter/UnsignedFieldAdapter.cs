using System.Reflection;

namespace Leafing.Data.Model.Member.Adapter
{
    internal class UnsignedFieldAdapter : FieldAdapter
    {
        public UnsignedFieldAdapter(FieldInfo fi)
            : base(fi)
        {
        }

        public override void SetValue(object obj, object value)
        {
            if (Info.FieldType == typeof(ulong))
            {
                base.SetValue(obj, (ulong)(long)value);
            }
            else if (Info.FieldType == typeof(uint))
            {
                base.SetValue(obj, (uint)(int)value);
            }
            else if (Info.FieldType == typeof(ushort))
            {
                base.SetValue(obj, (ushort)(short)value);
            }
        }
    }
}
