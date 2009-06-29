using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CreateTableListAttribute : Attribute
    {
        public Type[] Types;

        public CreateTableListAttribute(Type type)
        {
            Types = new Type[1];
            Types[0] = type;
        }

        public CreateTableListAttribute(Type type1, Type type2)
        {
            Types = new Type[2];
            Types[0] = type1;
            Types[1] = type2;
        }

        public CreateTableListAttribute(Type type1, Type type2, Type type3)
        {
            Types = new Type[3];
            Types[0] = type1;
            Types[1] = type2;
            Types[2] = type3;
        }

        public CreateTableListAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
