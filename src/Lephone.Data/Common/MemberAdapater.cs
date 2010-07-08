using System;
using System.Reflection;
using Lephone.Core;

namespace Lephone.Data.Common
{
    public abstract class MemberAdapter
    {
        #region MemberAdapter Impl

        internal class FieldAdapter : MemberAdapter
        {
            protected FieldInfo Info;

            public FieldAdapter(FieldInfo info)
            {
                this.Info = info;
            }

            public override Type MemberType
            {
                get
                {
                    return Info.FieldType;
                }
            }

            public override void SetValue(object obj, object value)
            {
                Info.SetValue(obj, value);
            }

            public override object GetValue(object obj)
            {
                return Info.GetValue(obj);
            }

            public override T[] GetAttributes<T>(bool inherit)
            {
                return ClassHelper.GetAttributes<T>(Info, inherit);
            }

            public override T GetAttribute<T>(bool inherit)
            {
                return ClassHelper.GetAttribute<T>(Info, inherit);
            }

            public override bool HasAttribute<T>(bool inherit)
            {
                return ClassHelper.HasAttribute<T>(Info, inherit);
            }

            public override string Name
            {
                get { return Info.Name; }
            }

            public override bool IsProperty
            {
                get { return false; }
            }

            public override MemberInfo GetMemberInfo()
            {
                return Info;
            }

            public override Type DeclaringType
            {
                get { return Info.DeclaringType; }
            }
        }

        internal class PropertyAdapter : MemberAdapter
        {
            protected PropertyInfo Info;

            public PropertyAdapter(PropertyInfo info)
            {
                this.Info = info;
            }

            public override Type MemberType
            {
                get { return Info.PropertyType; }
            }

            public override void SetValue(object obj, object value)
            {
                Info.SetValue(obj, value, null);
            }

            public override object GetValue(object obj)
            {
                return Info.GetValue(obj, null);
            }

            public override T[] GetAttributes<T>(bool inherit)
            {
                return ClassHelper.GetAttributes<T>(Info, inherit);
            }

            public override T GetAttribute<T>(bool inherit)
            {
                return ClassHelper.GetAttribute<T>(Info, inherit);
            }

            public override bool HasAttribute<T>(bool inherit)
            {
                return ClassHelper.HasAttribute<T>(Info, inherit);
            }

            public override string Name
            {
                get { return Info.Name; }
            }

            public override bool IsProperty
            {
                get { return true; }
            }

            public override MemberInfo GetMemberInfo()
            {
                return Info;
            }

            public override Type DeclaringType
            {
                get { return Info.DeclaringType; }
            }
        }

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

        #endregion

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

        public static MemberAdapter NewObject(FieldInfo fi)
        {
            if (fi.FieldType == typeof(ulong) || fi.FieldType == typeof(uint) || fi.FieldType == typeof(ushort))
            {
                return new UnsignedFieldAdapter(fi);
            }
            return new FieldAdapter(fi);
        }

        public static MemberAdapter NewObject(PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(ulong) || pi.PropertyType == typeof(uint) || pi.PropertyType == typeof(ushort))
            {
                return new UnsignedPropertyAdapter(pi);
            }
            return new PropertyAdapter(pi);
        }
    }
}
