using System;
using Lephone.Data.Model.Member.Adapter;

namespace Lephone.Data.Model.Member
{
    public class BooleanMemberHandler : MemberHandler
    {
        public BooleanMemberHandler(MemberAdapter fi)
            : base(fi)
        {
        }

        protected override void InnerSetValue(object obj, object value)
        {
            MemberInfo.SetValue(obj, Convert.ToBoolean(value));
        }
    }
}
