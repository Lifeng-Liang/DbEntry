using System.Collections.Generic;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public static class TypeHelper
    {
        public const string CompilerGenerated = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";

        public static List<CustomAttribute> GetCustomAttributes(this IMemberDefinition type, string attributeName)
        {
            var result = new List<CustomAttribute>();
            if (type.HasCustomAttributes)
            {
                foreach (CustomAttribute attribute in type.CustomAttributes)
                {
                    if (attribute.Constructor.DeclaringType.FullName != attributeName)
                        continue;

                    result.Add(attribute);
                }
            }
            return result;
        }

        public static CustomAttribute GetCustomAttribute(this IMemberDefinition type, string attributeName)
        {
            if (!type.HasCustomAttributes)
                return null;

            foreach (CustomAttribute attribute in type.CustomAttributes)
            {
                if (attribute.Constructor.DeclaringType.FullName != attributeName)
                    continue;

                return attribute;
            }

            return null;
        }

        public static bool IsCompilerGenerated(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(CompilerGenerated) != null;
        }

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
