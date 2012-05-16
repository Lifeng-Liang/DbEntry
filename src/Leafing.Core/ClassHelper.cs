using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Leafing.Core
{
	public static class ClassHelper
	{
        public static readonly BindingFlags AllFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
        public static readonly BindingFlags StaticFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
        public static readonly BindingFlags InstanceFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        public static readonly BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance;

        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType == typeof(Date))
            {
                return GetDate(value);
            }
            if (conversionType == typeof(Time))
            {
                return GetTime(value);
            }
            if (conversionType == typeof(Date?))
            {
                if (value == null) return null;
                return (Date?)(GetDate(value));
            }
            if (conversionType == typeof(Time?))
            {
                if (value == null) return null;
                return (Time?)(GetTime(value));
            }
            if(conversionType == typeof(DateTime?) && value == null)
            {
                return null;
            }
            if (conversionType.IsValueType && conversionType.IsGenericType)
            {
                object o = ChangeType(value, conversionType.GetGenericArguments()[0]);
                return CreateInstance(conversionType, o);
            }
            if (conversionType.IsEnum)
            {
                return value is string
                           ? Enum.Parse(conversionType, (string)value)
                           : Enum.ToObject(conversionType, value);
            }
            if (conversionType == typeof(bool))
            {
                if(value is bool)
                {
                    return (bool)value;
                }
                if(value == null)
                {
                    return false;
                }
                var vn = value.ToString().ToLower();
                if(vn == "on" || vn == "true" || vn == "1")
                {
                    return true;
                }
                return false;
            }
            return Convert.ChangeType(value, conversionType);
        }

	    private static Date GetDate(object value)
        {
            if (value is string)
            {
                return Date.Parse((string)value);
            }
            return new Date(Convert.ToDateTime(value));
        }

        private static Time GetTime(object value)
        {
            if (value is string)
            {
                return Time.Parse((string)value);
            }
            return new Time(Convert.ToDateTime(value));
        }

        public static ConstructorInfo GetArgumentlessConstructor(Type t)
        {
            var c = t.GetConstructor(InstancePublic, null, new Type[] { }, null);
            return c;
        }

        public static ConstructorInfo[] GetPublicOrProtectedConstructors(Type t)
        {
            var cs = t.GetConstructors(InstanceFlag);
            var list = new List<ConstructorInfo>();
            foreach(ConstructorInfo info in cs)
            {
                if(info.IsPublic || info.IsFamily)
                {
                    list.Add(info);
                }
            }
            return list.ToArray();
        }

        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        public static Func<object> GetConstructorDelegate(Type type)
        {
            var ci = GetArgumentlessConstructor(type);
            if(ci == null)
            {
                throw new CoreException("The type [" + type.Name + "] need a public paremeter-less constractor");
            }
            return GetConstructorDelegate(ci);
        }

        public static Func<object> GetConstructorDelegate(ConstructorInfo constructor)
        {
            return Expression.Lambda<Func<object>>(Expression.New(constructor)).Compile();
        }

        public static object CreateInstance(string className)
		{
			var t = Type.GetType(className, true);
			return CreateInstance(t);
		}

        public static object CreateInstance(string className, params object[] os)
		{
			var t = Type.GetType(className, true);
			return CreateInstance(t, os);
		}

		public static object CreateInstance(Assembly fromAssembly, string className)
		{
			var t = fromAssembly.GetType(className);
			return CreateInstance(t);
		}

        public static object CreateInstance(Assembly fromAssembly, string className, params object[] os)
		{
			var t = fromAssembly.GetType(className);
			return CreateInstance(t, os);
		}

		public static object CreateInstance(Type t)
		{
            return CreateInstance(t, new object[] { });
		}

        public static object CreateInstance(Type t, params object[] os)
        {
            var ts = GetTypesByObjs(os);
            var ci = t.GetConstructor(InstanceFlag, null, ts, null);
            return ci.Invoke(os);
        }

        public static T CreateInstance<T>(params object[] os)
        {
            return (T)CreateInstance(typeof(T), os);
        }

        public static void SetValue<T>(string fieldName, object value)
        {
            SetValue(typeof(T), fieldName, value);
        }

        public static void SetValue(Type sourceType, string fieldName, object value)
        {
            var fi = sourceType.GetField(fieldName, StaticFlag);
            if (fi != null)
                fi.SetValue(null, value);
        }

        public static void SetValue(object obj, string fieldName, object value)
        {
            var fi = obj.GetType().GetField(fieldName, AllFlag);
            if (fi != null)
                fi.SetValue(obj, value);
        }

        public static T GetValue<T>(string fieldName)
        {
            return (T)GetValue(typeof(T), fieldName);
        }

        public static object GetValue(Type sourceType, string fieldName)
        {
            var fi = sourceType.GetField(fieldName, StaticFlag);
            return fi == null ? null : fi.GetValue(null);
        }

        public static object GetValue(object obj, string fieldName)
        {
            var fi = obj.GetType().GetField(fieldName, AllFlag);
            return fi == null ? null : fi.GetValue(obj);
        }

        public static object CallFunction(Type ot, string functionName, params object[] os)
        {
            return CallFunction(null, ot, functionName, os);
        }

        public static object CallFunction(object obj, string functionName, params object[] os)
        {
            return CallFunction(obj, obj.GetType(), functionName, os);
        }

        private static object CallFunction(object obj, Type ot, string functionName, object[] os)
        {
            var ts = GetTypesByObjs(os);
            var mi = ot.GetMethod(functionName, AllFlag, null, CallingConventions.Any, ts, null);
            if (mi == null)
            {
                throw new SystemException(string.Format("Can not find the function called [{0}] in [{1}]", functionName, ot));
            }
            return mi.Invoke(obj, os);
        }

        public static Type[] GetTypesByObjs(params object[] os)
        {
            var ts = new Type[os.Length];
            for (int i = 0; i < os.Length; i++)
            {
                ts[i] = os[i].GetType();
            }
            return ts;
        }

        /// <summary>
        /// output a object with it's fields.
        /// </summary>
        /// <param name="o">the object</param>
        /// <param name="msg">just like "This object is : {0}"</param>
        /// <param name="spliter">fields spliter</param>
        /// <param name="nameTemplate">for field name output style</param>
        /// <param name="nullString">if field is null, output this string</param>
        /// <returns></returns>
        public static string ObjectToString(object o, string msg, string spliter, string nameTemplate, string nullString)
        {
            var sb = new StringBuilder();
            var fis = o.GetType().GetFields();
            var os = new object[fis.Length];

            for (int i = 0; i < fis.Length; i++)
            {
                if (nameTemplate != null)
                {
                    sb.AppendFormat(nameTemplate, fis[i].Name);
                }
                sb.Append("{");
                sb.Append(i.ToString());
                sb.Append("}");
                sb.Append(spliter);
                object oo = fis[i].GetValue(o);
                os[i] = (oo == null) ? nullString : oo.ToString();
            }
            if (fis.Length > 0)
            {
                sb.Length -= spliter.Length;
            }
            return string.Format(string.Format(msg, sb), os);
        }

        public static T[] GetAttributes<T>(this MethodInfo info, bool inherit) where T : Attribute
        {
            var os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(this MethodInfo info, bool inherit) where T : Attribute
        {
            var ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            return null;
        }

        public static T[] GetAttributes<T>(this FieldInfo info, bool inherit) where T : Attribute
        {
            var os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(this FieldInfo info, bool inherit) where T : Attribute
        {
            var ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            return null;
        }

        public static T[] GetAttributes<T>(this PropertyInfo info, bool inherit) where T : Attribute
        {
            var os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(this PropertyInfo info, bool inherit) where T : Attribute
        {
            var ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            return null;
        }

        public static T[] GetAttributes<T>(this Type info, bool inherit) where T : Attribute
        {
            var os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(this Type info, bool inherit) where T : Attribute
        {
            var ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            return null;
        }

        public static T[] GetAttributes<T>(this ParameterInfo info, bool inherit) where T : Attribute
        {
            var os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(this ParameterInfo info, bool inherit) where T : Attribute
        {
            var ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            return null;
        }

        public static bool HasAttribute<T>(this MethodInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static bool HasAttribute<T>(this FieldInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static bool HasAttribute<T>(this PropertyInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static bool HasAttribute<T>(this Type info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static bool HasAttribute<T>(this ParameterInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static bool IsChildOf(this Type tc, Type tf)
        {
            if(tf.IsInterface)
            {
                return tc.GetInterfaces().Any(type => type.Equals(tf));
            }
            return tc.IsSubclassOf(tf);
        }
	}
}
