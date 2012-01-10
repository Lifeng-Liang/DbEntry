using System;
using System.Collections.Generic;
using System.Reflection;
using Leafing.Core;
using Leafing.Core.Ioc;
using Leafing.Data;
using Leafing.Data.Definition;

namespace Leafing.Web.Mvc.Core
{
    [DependenceEntry, Implementation(1)]
    public class TypeBinder
    {
        public static readonly TypeBinder Instance = SimpleContainer.Get<TypeBinder>();

        protected static Dictionary<Type, int> Types;
        protected static Dictionary<Type, Type> NullableTypes;

        static TypeBinder()
        {
            Types = new Dictionary<Type, int>
                        {
                            {typeof(int), 1},
                            {typeof(long), 1},
                            {typeof(bool), 1},
                            {typeof(DateTime), 1},
                            {typeof(float), 1},
                            {typeof(double), 1},
                            {typeof(Date), 1},
                            {typeof(Time), 1},
                            {typeof(string), 1}
                        };
            NullableTypes = new Dictionary<Type, Type>
                                {
                                    {typeof(int?), typeof(int)},
                                    {typeof(long?), typeof(long)},
                                    {typeof(bool?), typeof(bool)},
                                    {typeof(DateTime?), typeof(DateTime)},
                                    {typeof(float?), typeof(float)},
                                    {typeof(double?), typeof(double)},
                                    {typeof(Date?), typeof(Date)},
                                    {typeof(Time?), typeof(Time)},
                                };
        }

        public virtual object GetObject(string name, Type type)
        {
            if(type.IsEnum)
            {
                return ProcessEnum(name, type);
            }
            if(Types.ContainsKey(type))
            {
                return ProcessSimpleType(name, type);
            }
            if (NullableTypes.ContainsKey(type))
            {
                return ProcessSimpleNullableType(name, NullableTypes[type]);
            }
            if(type.IsValueType)
            {
                throw new NotSupportedException(string.Format("The type [{0}] is not supported", type));
            }
            if(type.GetInterface(typeof(IDbObject).FullName) != null)
            {
                return ProcessModel(type);
            }
            return ProcessOtherType(name, type);
        }

        private object ProcessEnum(string name, Type type)
        {
            var value = HttpContextHandler.Instance[name];
            return Enum.Parse(type, value);
        }

        private object ProcessSimpleNullableType(string name, Type type)
        {
            var value = HttpContextHandler.Instance[name];
            if(value.IsNullOrEmpty())
            {
                return null;
            }
            return ClassHelper.ChangeType(value, type);
        }

        protected virtual object ProcessSimpleType(string name, Type type)
        {
            var value = HttpContextHandler.Instance[name];
            return ClassHelper.ChangeType(value, type);
        }

        protected virtual object ProcessModel(Type type)
        {
            var ctx = ModelContext.GetInstance(type);
            object obj;
            if(HttpContextHandler.Instance["Id"].IsNullOrEmpty())
            {
                obj = ctx.NewObject();
            }
            else
            {
                var key = HttpContextHandler.Instance["Id"];
                var k = Convert.ChangeType(key, ctx.Info.KeyMembers[0].MemberType);
                obj = ctx.Operator.GetObject(k);
            }
            UpdateSimpleMembers(obj, ctx);
            return obj;
        }

        private static void UpdateSimpleMembers(object obj, ModelContext ctx)
        {
            foreach(var field in ctx.Info.SimpleMembers)
            {
                if(!field.Is.Key)
                {
                    var value = HttpContextHandler.Instance[field.MemberInfo.Name];
                    if(value != null || field.MemberType == typeof(bool))
                    {
                        if (value == "" && field.Is.AllowNull)
                        {
                            field.SetValue(obj, null);
                        }
                        else
                        {
                            field.SetValue(obj, ClassHelper.ChangeType(value, field.MemberType));
                        }
                    }
                }
            }
        }

        protected virtual object ProcessOtherType(string name, Type type)
        {
            var obj = ClassHelper.CreateInstance(type);
            foreach (var info in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = HttpContextHandler.Instance[info.Name];
                var av = ClassHelper.ChangeType(value, info.FieldType);
                info.SetValue(obj, av);
            }
            foreach (var info in type.GetProperties())
            {
                var value = HttpContextHandler.Instance[info.Name];
                var av = ClassHelper.ChangeType(value, info.PropertyType);
                info.SetValue(obj, av, null);
            }
            return obj;
        }
    }
}
