using System;
using System.Reflection;

namespace Leafing.Data.Model.Member.Adapter {
    public abstract class MemberAdapter {
        public abstract bool IsProperty { get; }
        public abstract string Name { get; }
        public abstract T[] GetAttributes<T>(bool inherit) where T : Attribute;
        public abstract T GetAttribute<T>(bool inherit) where T : Attribute;
        public abstract bool HasAttribute<T>(bool inherit) where T : Attribute;
        public abstract Type MemberType { get; }
        public abstract void SetValue(object obj, object value);
        public abstract object GetValue(object obj);
        public abstract MemberInfo GetMemberInfo();
        public abstract Type DeclaringType { get; }

        public static MemberAdapter NewObject(FieldInfo fi) {
            if (fi.FieldType == typeof(ulong) || fi.FieldType == typeof(uint) || fi.FieldType == typeof(ushort)) {
                return new UnsignedFieldAdapter(fi);
            }
            return new FieldAdapter(fi);
        }

        public static MemberAdapter NewObject(PropertyInfo pi) {
            if (pi.PropertyType == typeof(ulong) || pi.PropertyType == typeof(uint) || pi.PropertyType == typeof(ushort)) {
                return new UnsignedPropertyAdapter(pi);
            }
            return new PropertyAdapter(pi);
        }
    }
}