using System;
using System.Linq;
using Mono.Cecil;

namespace Lephone.Processor
{
    public static class TypeHelper
    {
        public static CustomAttribute GetCustomAttribute(MethodDefinition type, Type attributeType)
        {
            if (!type.HasCustomAttributes)
                return null;

            foreach (CustomAttribute attribute in type.CustomAttributes)
            {
                if (attribute.Constructor.DeclaringType.FullName != attributeType.FullName)
                    continue;

                return attribute;
            }

            return null;
        }


        public static TypeDefinition InterfaceDef;

        public static bool IsDbModel(TypeDefinition type)
        {
            //if(type.IsClass)
            //{
            //    if(type.HasInterfaces)
            //    {
            //        if(type.FullName == "System.Object")
            //        {
            //            return false;
            //        }
            //        var b = type.Interfaces.Any(p => p.FullName == "Lephone.Data.Definition.IDbObject");
            //        if(b)
            //        {
            //            return true;
            //        }
            //        return IsDbModel(type.BaseType.);
            //    }
            //}
            return true;
        }
    }
}
