using System;
using System.Reflection;
using Leafing.Data.Common;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data.Model.Member {
    public class NullableMemberHandler : MemberHandler {
        protected readonly ConstructorInfo Ctor;
        protected readonly PropertyInfo NullableField;

        public NullableMemberHandler(MemberAdapter fi)
            : base(fi) {
            Ctor = NullableHelper.GetConstructorInfo(fi.MemberType);
            NullableField = fi.MemberType.GetProperty("Value");
        }

        protected override void InnerSetValue(object obj, object value) {
            if (value != null) {
                if (NullableField.PropertyType == typeof(Date) && value.GetType() == typeof(DateTime)) {
                    value = (Date)(DateTime)value;
                } else if (NullableField.PropertyType == typeof(Time) && value.GetType() == typeof(DateTime)) {
                    value = (Time)(DateTime)value;
                } else if (NullableField.PropertyType == typeof(Guid) && value.GetType() != typeof(Guid)) {
                    value = new Guid(value.ToString());
                }
                object oo = Ctor.Invoke(new[] { value });
                MemberInfo.SetValue(obj, oo);
            } else {
                MemberInfo.SetValue(obj, null);
            }
        }

        public override object GetValue(object obj) {
            object r = MemberInfo.GetValue(obj);
            if (r == null) {
                return null;
            }
            return NullableField.GetValue(r, null);
        }
    }
}