
#region usings

using System;
using System.Reflection;
using System.Text;

#endregion

namespace Lephone.Util
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
            else if (conversionType == typeof(Time))
            {
                return GetTime(value);
            }
            else if (conversionType == typeof(Date?))
            {
                if (value == null) return null;
                return new Date?(GetDate(value));
            }
            else if (conversionType == typeof(Time?))
            {
                if (value == null) return null;
                return new Time?(GetTime(value));
            }
            else
            {
                return Convert.ChangeType(value, conversionType);
            }
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

        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

		public static object CreateInstance(string ClassName)
		{
			Type t = Type.GetType(ClassName, true);
			return CreateInstance(t);
		}

        public static object CreateInstance(string ClassName, params object[] os)
		{
			Type t = Type.GetType(ClassName, true);
			return CreateInstance(t, os);
		}

		public static object CreateInstance(Assembly FromAssembly, string ClassName)
		{
			Type t = FromAssembly.GetType(ClassName);
			return CreateInstance(t);
		}

        public static object CreateInstance(Assembly FromAssembly, string ClassName, params object[] os)
		{
			Type t = FromAssembly.GetType(ClassName);
			return CreateInstance(t, os);
		}

		public static object CreateInstance(Type t)
		{
            return CreateInstance(t, new object[] { });
		}

        public static object CreateInstance(Type t, params object[] os)
        {
            Type[] ts = GetTypesByObjs(os);
            ConstructorInfo ci = t.GetConstructor(InstanceFlag, null, ts, null);
            return ci.Invoke(os);
        }

        public static T CreateInstance<T>(params object[] os)
        {
            return (T)CreateInstance(typeof(T), os);
        }

        public static void SetValue<T>(string FieldName, object value)
        {
            SetValue(typeof(T), FieldName, value);
        }

        public static void SetValue(Type SourceType, string FieldName, object value)
        {
            FieldInfo fi = SourceType.GetField(FieldName, StaticFlag);
            if (fi != null)
                fi.SetValue(null, value);
        }

        public static void SetValue(object obj, string FieldName, object value)
        {
            FieldInfo fi = obj.GetType().GetField(FieldName, AllFlag);
            if (fi != null)
                fi.SetValue(obj, value);
        }

        public static T GetValue<T>(string FieldName)
        {
            return (T)GetValue(typeof(T), FieldName);
        }

        public static object GetValue(Type SourceType, string FieldName)
        {
            FieldInfo fi = SourceType.GetField(FieldName, StaticFlag);
            return fi == null ? null : fi.GetValue(null);
        }

        public static object GetValue(object obj, string FieldName)
        {
            FieldInfo fi = obj.GetType().GetField(FieldName, AllFlag);
            return fi == null ? null : fi.GetValue(obj);
        }

        public static object CallFunction(Type ot, string FunctionName, params object[] os)
        {
            return CallFunction(null, ot, FunctionName, os);
        }

        public static object CallFunction(object obj, string FunctionName, params object[] os)
        {
            return CallFunction(obj, obj.GetType(), FunctionName, os);
        }

        private static object CallFunction(object obj, Type ot, string FunctionName, object[] os)
        {
            Type[] ts = GetTypesByObjs(os);
            MethodInfo mi = ot.GetMethod(FunctionName, AllFlag, null, CallingConventions.Any, ts, null);
            if (mi == null)
            {
                throw new SystemException(string.Format("Can not find the function called [{0}] in [{1}]", FunctionName, ot));
            }
            return mi.Invoke(obj, os);
        }

        public static Type[] GetTypesByObjs(params object[] os)
        {
            Type[] ts = new Type[os.Length];
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
        /// <param name="NameTemplate">for field name output style</param>
        /// <param name="NullString">if field is null, output this string</param>
        /// <returns></returns>
        public static string ObjectToString(object o, string msg, string spliter, string NameTemplate, string NullString)
        {
            StringBuilder sb = new StringBuilder();
            System.Reflection.FieldInfo[] fis = o.GetType().GetFields();
            object[] os = new object[fis.Length];

            for (int i = 0; i < fis.Length; i++)
            {
                if (NameTemplate != null)
                {
                    sb.AppendFormat(NameTemplate, fis[i].Name);
                }
                sb.Append("{");
                sb.Append(i.ToString());
                sb.Append("}");
                sb.Append(spliter);
                object oo = fis[i].GetValue(o);
                os[i] = (oo == null) ? NullString : oo.ToString();
            }
            if (fis.Length > 0)
            {
                sb.Length -= spliter.Length;
            }
            return string.Format(string.Format(msg, sb.ToString()), os);
        }

        public static T[] GetAttributes<T>(MethodInfo info, bool inherit) where T : Attribute
        {
            object[] os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(MethodInfo info, bool inherit) where T : Attribute
        {
            T[] ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            else
                return null;
        }

        public static bool HasAttribute<T>(MethodInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static T[] GetAttributes<T>(FieldInfo info, bool inherit) where T : Attribute
        {
            object[] os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(FieldInfo info, bool inherit) where T : Attribute
        {
            T[] ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            else
                return null;
        }

        public static bool HasAttribute<T>(FieldInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static T[] GetAttributes<T>(PropertyInfo info, bool inherit) where T : Attribute
        {
            object[] os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(PropertyInfo info, bool inherit) where T : Attribute
        {
            T[] ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            else
                return null;
        }

        public static bool HasAttribute<T>(PropertyInfo info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }

        public static T[] GetAttributes<T>(Type info, bool inherit) where T : Attribute
        {
            object[] os = info.GetCustomAttributes(typeof(T), inherit);
            return (T[])os;
        }

        public static T GetAttribute<T>(Type info, bool inherit) where T : Attribute
        {
            T[] ts = GetAttributes<T>(info, inherit);
            if (ts != null && ts.Length > 0)
                return ts[0];
            else
                return null;
        }

        public static bool HasAttribute<T>(Type info, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(info, inherit) != null;
        }
    }
}
