using System;
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

        public static bool IsExclude(this IMemberDefinition type)
        {
            return type.GetCustomAttribute("Lephone.Data.Definition.ExcludeAttribute") != null;
        }

        public static MethodDefinition GetMethod(this TypeReference type, string name)
        {
            var realType = type.Resolve();
            foreach (var method in realType.Methods)
            {
                if (method.Name == name)
                {
                    return method;
                }
            }
            if (type.FullName == "System.Object")
            {
                return null;
            }
            return GetMethod(realType.BaseType, name);
        }

        public static string GetColumnName(this PropertyDefinition property)
        {
            var dbColumn = property.GetCustomAttribute("Lephone.Data.Definition.DbColumnAttribute");
            if (dbColumn != null)
            {
                return dbColumn.ConstructorArguments[0].Value.ToString();
            }
            return property.Name;
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

        public static TypeReference MakeGenericType(TypeReference type, params TypeReference[] arguments)
        {
            if (type.GenericParameters.Count != arguments.Length)
                throw new ArgumentException();

            var instance = new GenericInstanceType(type);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);

            return instance;

        }
    }
}
