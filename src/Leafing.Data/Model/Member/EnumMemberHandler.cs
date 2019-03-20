using System;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data.Model.Member {
    public class EnumMemberHandler : MemberHandler {
        public EnumMemberHandler(MemberAdapter fi)
            : base(fi) {
        }

        protected override void InnerSetValue(object obj, object value) {
            MemberInfo.SetValue(obj, Enum.Parse(MemberInfo.MemberType, value.ToString()));
        }
    }
}