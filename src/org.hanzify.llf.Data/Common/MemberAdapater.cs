
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace org.hanzify.llf.Data.Common
{
    internal abstract class MemberAdapter
    {
        #region MemberAdapter Impl

        internal class FieldAdapter : MemberAdapter
        {
            private FieldInfo fi;

            public FieldAdapter(FieldInfo fi)
            {
                this.fi = fi;
            }

            public override Type MemberType
            {
                get
                {
                    return fi.FieldType;
                }
            }

            public override void SetValue(object obj, object value)
            {
                fi.SetValue(obj, value);
            }

            public override object GetValue(object obj)
            {
                return fi.GetValue(obj);
            }

            public override object[] GetCustomAttributes(Type t, bool inherit)
            {
                return fi.GetCustomAttributes(t, inherit);
            }

            public override string Name
            {
                get { return fi.Name; }
            }

            public override bool IsProperty
            {
                get { return false; }
            }

            public override MemberInfo GetMemberInfo()
            {
                return fi;
            }

            public override void EmitSet(ILGenerator il)
            {
                il.Emit(OpCodes.Stfld, fi);
            }

            public override void EmitGet(ILGenerator il)
            {
                il.Emit(OpCodes.Ldfld, fi);
            }
        }

        internal class PropertyAdapter : MemberAdapter
        {
            private PropertyInfo pi;

            public PropertyAdapter(PropertyInfo pi)
            {
                this.pi = pi;
            }

            public override Type MemberType
            {
                get
                {
                    return pi.PropertyType;
                }
            }

            public override void SetValue(object obj, object value)
            {
                pi.SetValue(obj, value, null);
            }

            public override object GetValue(object obj)
            {
                return pi.GetValue(obj, null);
            }

            public override object[] GetCustomAttributes(Type t, bool inherit)
            {
                return pi.GetCustomAttributes(t, inherit);
            }

            public override string Name
            {
                get { return pi.Name; }
            }

            public override bool IsProperty
            {
                get { return true; }
            }

            public override MemberInfo GetMemberInfo()
            {
                return pi;
            }

            public override void EmitSet(ILGenerator il)
            {
                il.Emit(OpCodes.Callvirt, pi.GetSetMethod());
            }

            public override void EmitGet(ILGenerator il)
            {
                il.Emit(OpCodes.Callvirt, pi.GetGetMethod());
            }
        }

        #endregion

        public abstract bool IsProperty { get; }
        public abstract string Name { get; }
        public abstract object[] GetCustomAttributes(Type t, bool inherit);
        public abstract Type MemberType { get; }
        public abstract void SetValue(object obj, object value);
        public abstract object GetValue(object obj);
        public abstract MemberInfo GetMemberInfo();
        public abstract void EmitSet(ILGenerator il);
        public abstract void EmitGet(ILGenerator il);

        public static MemberAdapter NewObject(FieldInfo fi)
        {
            return new FieldAdapter(fi);
        }

        public static MemberAdapter NewObject(PropertyInfo pi)
        {
            return new PropertyAdapter(pi);
        }
    }
}
