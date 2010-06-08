using System;

namespace Lephone.Data.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModelHandlerAttribute : Attribute
    {
        public Type Type;

        public ModelHandlerAttribute(Type type)
        {
            Type = type;
        }
    }
}
