
#region usings

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace Lephone.Util
{
    public delegate void CallbackVoidHandler();
    public delegate void CallbackObjectHandler<T>(T o);
    public delegate TR CallbackHandler<T, TR>(T o);
    public delegate void CallbackObjectHandler2<T, T1>(T o, T1 o1);

    public static class CommonHelper
    {
        public static void IfCatchException(bool CatchException, CallbackVoidHandler callback)
        {
            if (CatchException)
            {
                try
                {
                    callback();
                }
                catch { }
            }
            else
            {
                callback();
            }
        }

        public static void TryEnumerate(object obj, CallbackObjectHandler<object> callback)
        {
            if (obj != null)
            {
                if (obj is IEnumerable)
                {
                    foreach (object o in (IEnumerable)obj)
                    {
                        callback(o);
                    }
                }
                else
                {
                    callback(obj);
                }
            }
        }

        public static object GetEmptyValue(Type t)
        {
            return GetEmptyValue(t, true, "Unknown Type");
        }

        public static object GetEmptyValue(Type t, bool IncludeDateTime, string ExceptionText)
        {
            if (t == typeof(int))
            {
                return 0;
            }
            else if (t == typeof(long))
            {
                return 0L;
            }
            else if (IncludeDateTime && t == typeof(DateTime))
            {
                return DateTime.MinValue;
            }
            else if (t == typeof(Guid))
            {
                return Guid.Empty;
            }
            else
            {
                throw new NotSupportedException(ExceptionText);
            }
        }

        public static List<T> NewList<T>(params T[] ts)
        {
            List<T> ret = new List<T>();
            foreach (T t in ts)
            {
                ret.Add(t);
            }
            return ret;
        }
    }
}
