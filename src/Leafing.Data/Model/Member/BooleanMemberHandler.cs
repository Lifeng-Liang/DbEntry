using System;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data.Model.Member {
    public class BooleanMemberHandler : MemberHandler {
        public BooleanMemberHandler(MemberAdapter fi)
            : base(fi) {
        }

        protected override void InnerSetValue(object obj, object value) {
            MemberInfo.SetValue(obj, Convert.ToBoolean(value));
        }
    }
}