using System;

namespace Lephone.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class BindAttribute : Attribute
    {
        public Type BinderType;

        public BindAttribute()
        {
        }

        public BindAttribute(Type binderType)
        {
            BinderType = binderType;
        }
    }
}
