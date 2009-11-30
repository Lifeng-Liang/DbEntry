using System;

namespace Lephone.Data.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ForTypeAttribute : Attribute
    {
        public Type Type;

        public ForTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
