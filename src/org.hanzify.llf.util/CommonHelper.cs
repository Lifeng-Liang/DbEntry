
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
        public static void Assert(bool AssertCondition)
        {
            Assert(AssertCondition, "");
        }

        public static void Assert(bool AssertCondition, string FailedMessage, params object[] os)
        {
            if (!AssertCondition)
            {
                string s = string.Format(FailedMessage, os);
                throw new LephoneException(s);
            }
        }

        public static int main(string[] args, int minArgCount, string Usage, CallbackVoidHandler Callback)
        {
            if (args.Length >= minArgCount)
            {
                try
                {
                    Callback();
                    return 0;
                }
                catch (Exception ex)
                {
                    if (args.Length > minArgCount && args[args.Length - 1] == "-m")
                    {
                        Console.WriteLine(ex);
                    }
                    else
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return 2;
                }
            }
            else
            {
                Console.WriteLine(Usage);
                return 1;
            }
        }

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
            else if (IncludeDateTime && t == typeof(Date))
            {
                return new Date(DateTime.MinValue);
            }
            else if (IncludeDateTime && t == typeof(Time))
            {
                return new Time(DateTime.MinValue);
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
