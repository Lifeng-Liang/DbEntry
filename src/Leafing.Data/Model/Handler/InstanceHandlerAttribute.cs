using System;

namespace Leafing.Data.Model.Handler
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class InstanceHandlerAttribute : Attribute
    {
        public Type Type;

        public InstanceHandlerAttribute(Type type)
        {
            Type = type;
        }
    }
}

