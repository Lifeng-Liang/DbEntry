using System;
using Lephone.Data.Model.Member.Adapter;

namespace Lephone.Data.Model.Member
{
    public class NullableBooleanMemberHandler : NullableMemberHandler
    {
        public NullableBooleanMemberHandler(MemberAdapter fi)
            : base(fi)
        {
        }

        protected override void InnerSetValue(object obj, object value)
        {
            object oo = Ctor.Invoke(new object[] { Convert.ToBoolean(value) });
            MemberInfo.SetValue(obj, oo);
        }
    }
}
