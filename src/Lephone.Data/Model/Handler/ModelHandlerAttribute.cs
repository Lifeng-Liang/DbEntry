using System;

namespace Lephone.Data.Model.Handler
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

