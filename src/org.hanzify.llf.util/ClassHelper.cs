
#region usings

using System;
using System.Reflection;
using System.Text;

#endregion

namespace org.hanzify.llf.util
{
	public static class ClassHelper
	{
        public static readonly BindingFlags AllFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
        public static readonly BindingFlags StaticFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
        public static readonly BindingFlags InstanceFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        public static readonly BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance;

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
            fi.SetValue(null, value);
        }

        public static void SetValue(object obj, string FieldName, object value)
        {
            FieldInfo fi = obj.GetType().GetField(FieldName, AllFlag);
            fi.SetValue(obj, value);
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
    }
}
