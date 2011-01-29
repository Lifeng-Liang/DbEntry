using System;
using Lephone.Data.Model.Member.Adapter;

namespace Lephone.Data.Model.Member
{
    public class EnumMemberHandler : MemberHandler
    {
        public EnumMemberHandler(MemberAdapter fi)
            : base(fi)
        {
        }

        protected override void InnerSetValue(object obj, object value)
        {
            MemberInfo.SetValue(obj, Enum.Parse(MemberInfo.MemberType, value.ToString()));
        }
    }
}
