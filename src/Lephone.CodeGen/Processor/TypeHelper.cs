using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public static class TypeHelper
    {
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

        public static object GetField(this CustomAttribute attribute, string name)
        {
            foreach (var field in attribute.Fields)
            {
                if(field.Name == name)
                {
                    return field.Argument.Value;
                }
            }
            return null;
        }

        public static bool IsCompilerGenerated(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.CompilerGeneratedAttribute) != null;
        }

        public static bool IsExclude(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.ExcludeAttribute) != null;
        }

        public static bool IsDbKey(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.DbKeyAttribute) != null;
        }

        public static bool IsAllowNull(this IMemberDefinition type)
        {
            if (type.Name == "Nullable`1")
            {
                return true;
            }
            return type.GetCustomAttribute(KnownTypesHandler.AllowNullAttribute) != null;
        }

        public static MethodDefinition GetMethod(this TypeReference type, string name, params Type[] types)
        {
            var realType = type.Resolve();
            foreach (var method in realType.Methods)
            {
                if (method.Name == name)
                {
                    if(types.Length == 0)
                    {
                        return method;
                    }
                    var pl = method.HasParameters ? method.Parameters.Count : 0;
                    if(pl == types.Length)
                    {
                        bool find = true;
                        for (int i = 0; i < pl; i++)
                        {
                            if(method.Parameters[i].ParameterType.FullName != types[i].FullName)
                            {
                                find = false;
                                break;
                            }
                        }
                        if(find)
                        {
                            return method;
                        }
                    }
                }
            }
            if (type.FullName == KnownTypesHandler.Object)
            {
                return null;
            }
            return GetMethod(realType.BaseType, name);
        }

        public static MethodDefinition GetConstructor(this TypeReference type, params Type[] types)
        {
            foreach (var method in type.Resolve().Methods)
            {
                if(method.IsConstructor)
                {
                    if(IsParametersSame(method, types))
                    {
                        return method;
                    }
                }
            }
            throw new ApplicationException("Can not find ctor");
        }

        private static bool IsParametersSame(MethodDefinition ctor, Type[] types)
        {
            if(ctor.Parameters.Count == types.Length)
            {
                for (int i = 0; i < ctor.Parameters.Count; i++)
                {
                    if(ctor.Parameters[i].ParameterType.FullName != types[i].FullName)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static string GetColumnName(this PropertyDefinition property)
        {
            var dbColumn = property.GetCustomAttribute(KnownTypesHandler.DbColumnAttribute);
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
            {
                throw new ArgumentException();
            }

            var instance = new GenericInstanceType(type);
            foreach (var argument in arguments)
            {
                instance.GenericArguments.Add(argument);
            }

            return instance;
        }

        public static bool BaseTypeIsDbObjectSmartUpdate(this TypeReference type)
        {
            if(type.FullName == KnownTypesHandler.DbObjectSmartUpdate)
            {
                return true;
            }
            if(type.FullName == KnownTypesHandler.Object)
            {
                return false;
            }
            return BaseTypeIsDbObjectSmartUpdate(type.Resolve().BaseType);
        }
    }
}
