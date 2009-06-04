using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CreateTableListAttribute : Attribute
    {
        public Type[] Types;

        public CreateTableListAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
