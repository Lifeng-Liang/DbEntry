using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Core;
using Lephone.Data.Definition;

namespace Lephone.Web.Mvc
{
    public class TypeBinder
    {
        public static readonly TypeBinder Instance = (TypeBinder)ClassHelper.CreateInstance(WebSettings.TypeBinder);

        protected static Dictionary<Type, int> Types;

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
        }

        public virtual object GetObject(string name, Type type)
        {
            if(Types.ContainsKey(type))
            {
                return ProcessSimpleType(name, type);
            }
            if(type.GetInterface(typeof(IDbObject).FullName) != null)
            {
                return ProcessModel(type);
            }
            return ProcessOtherType(name, type);
        }

        protected virtual object ProcessSimpleType(string name, Type type)
        {
            var request = System.Web.HttpContext.Current.Request;
            var value = request[name];
            return ClassHelper.ChangeType(value, type);
        }

        protected virtual object ProcessModel(Type type)
        {
            var obj = (IDbObject)ClassHelper.CreateInstance(type);
            obj.ParseFromRequst();
            return obj;
        }

        protected virtual object ProcessOtherType(string name, Type type)
        {
            var request = System.Web.HttpContext.Current.Request;
            var obj = ClassHelper.CreateInstance(type);
            foreach (var info in type.GetFields(BindingFlags.Public))
            {
                var value = request[info.Name];
                var av = ClassHelper.ChangeType(value, info.FieldType);
                info.SetValue(obj, av);
            }
            foreach (var info in type.GetProperties())
            {
                var value = request[info.Name];
                var av = ClassHelper.ChangeType(value, info.PropertyType);
                info.SetValue(obj, av, null);
            }
            return obj;
        }
    }
}
