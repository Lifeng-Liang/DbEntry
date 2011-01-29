using System.Reflection;

namespace Lephone.Data.Model.Member.Adapter
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
                Info.SetValue(obj, (ulong)(long)value);
            }
            else if (Info.FieldType == typeof(uint))
            {
                Info.SetValue(obj, (uint)(int)value);
            }
            else if (Info.FieldType == typeof(ushort))
            {
                Info.SetValue(obj, (ushort)(short)value);
            }
        }
    }
}
