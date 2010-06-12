using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class ModelMember
    {
        //TODO: complates this
        public bool IsDbKey;
        public TypeReference MemberType;
        public MethodReference Constructor;
        public bool AllowNull;
        public IMemberDefinition Member;

        public ModelMember(IMemberDefinition member, TypeReference memberType)
        {
            Member = member;
            MemberType = memberType;
            Initialize();
        }

        private void Initialize()
        {
            IsDbKey = Member.IsDbKey();
            AllowNull = Member.IsAllowNull();
            if(!MemberType.IsValueType)
            {
                Constructor = MemberType.GetConstructor();
            }
        }
    }
}
