using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace Lephone.Processor
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

        public static CustomAttribute GetCustomAttribute(this ICustomAttributeProvider type, string attributeName)
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

        public static CustomAttribute GetDbColumnAttribute(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.DbColumnAttribute);
        }

        public static bool IsCompilerGenerated(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.CompilerGeneratedAttribute) != null;
        }

        public static bool IsExclude(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.ExcludeAttribute) != null;
        }

        public static bool IsAssemblyProcessed(this ModuleDefinition module)
        {
            return module.GetCustomAttribute(KnownTypesHandler.AssemblyProcessed) != null;
        }

        public static bool DontNeedToDoAnything(this ModuleDefinition module)
        {
            foreach (var reference in module.AssemblyReferences)
            {
                if(reference.FullName == KnownTypesHandler.LephoneData)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsHandlerExclude(this PropertyDefinition type)
        {
            if(type.GetCustomAttribute(KnownTypesHandler.ExcludeAttribute) != null)
            {
                return true;
            }
            if(type.GetCustomAttribute(KnownTypesHandler.HasOneAttribute) != null)
            {
                return true;
            }
            if (type.GetCustomAttribute(KnownTypesHandler.HasManyAttribute) != null)
            {
                return true;
            }
            if (type.GetCustomAttribute(KnownTypesHandler.HasAndBelongsToManyAttribute) != null)
            {
                return true;
            }
            if (type.GetCustomAttribute(KnownTypesHandler.BelongsToAttribute) != null)
            {
                return true;
            }
            if (type.GetCustomAttribute(KnownTypesHandler.LazyLoadAttribute) != null)
            {
                return true;
            }
            return false;
        }

        public static CustomAttribute GetDbKey(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.DbKeyAttribute);
        }

        public static bool IsSpecialName(this IMemberDefinition type)
        {
            return type.GetCustomAttribute(KnownTypesHandler.SpecialNameAttribute) != null;
        }

        public static bool IsAllowNull(this IMemberDefinition type)
        {
            if (type.Name == "Nullable`1")
            {
                return true;
            }
            return type.GetCustomAttribute(KnownTypesHandler.AllowNullAttribute) != null;
        }

        public static MethodReference GetMethod(this TypeReference type, string name, params Type[] types)
        {
            var method = InnerGetMethod(type, name, types);
            if(type is GenericInstanceType)
            {
                var m = new MethodReference(method.Name, method.ReturnType)
                            {
                                DeclaringType = type,
                                HasThis = method.HasThis,
                                ExplicitThis = method.ExplicitThis
                            };
                foreach(var p in method.Parameters)
                {
                    m.Parameters.Add(p);
                }
                method = m;
            }
            return method;
        }

        private static MethodReference InnerGetMethod(TypeReference type, string name, params Type[] types)
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
            if(types.Length == 0)
            {
                return true;
            }
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
