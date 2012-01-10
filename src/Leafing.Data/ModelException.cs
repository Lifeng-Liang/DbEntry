using System;
using System.Runtime.Serialization;
using Leafing.Data.Model.Member.Adapter;

namespace Leafing.Data
{
    [Serializable]
    public class ModelException : DataException
    {
        public ModelException(Type dbObjectType, string errorMessage) : base(GetMessage(dbObjectType, errorMessage)) { }
        public ModelException(MemberAdapter member, string errorMessage) : base(GetMessage(member, errorMessage)) { }
        protected ModelException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ModelException(string message, Exception innerException) : base(message, innerException) { }

        private static string GetMessage(Type dbObjectType, string errorMessage)
        {
            var result = string.Format("[{0}]{1}", dbObjectType, errorMessage);
            return result;
        }

        private static string GetMessage(MemberAdapter member, string errorMessage)
        {
            var result = string.Format("[{0}.{1}]{2}", member.DeclaringType.Name, member.Name, errorMessage);
            return result;
        }
    }
}
