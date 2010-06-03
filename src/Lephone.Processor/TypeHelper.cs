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

        public static bool IsDbModel(this TypeDefinition type)
        {
            return type.Interfaces.Any(p => p.FullName == "Lephone.Data.Definition.IDbObject");
        }
    }
}
