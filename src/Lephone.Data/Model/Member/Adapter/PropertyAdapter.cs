using System;
using System.Reflection;
using Lephone.Core;

namespace Lephone.Data.Model.Member.Adapter
{
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
            return Info.GetAttributes<T>(inherit);
        }

        public override T GetAttribute<T>(bool inherit)
        {
            return Info.GetAttribute<T>(inherit);
        }

        public override bool HasAttribute<T>(bool inherit)
        {
            return Info.HasAttribute<T>(inherit);
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
}
