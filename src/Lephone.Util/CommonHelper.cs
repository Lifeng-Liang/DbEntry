using System;
using System.Collections;
using System.Collections.Generic;

namespace Lephone.Util
{
    public delegate void CallbackVoidHandler();
    public delegate void CallbackObjectHandler<T>(T o);
    public delegate TR CallbackReturnHandler<T, TR>(T o);
    public delegate TR CallbackReturnHandler2<T1, T2, TR>(T1 o1, T2 o2);
    public delegate void CallbackObjectHandler2<T1, T2>(T1 o1, T2 o2);

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

        public static bool AreEqual(object o1, object o2)
        {
            if(o1 == null && o2 == null) { return true; }
            if (o1 == null || o2 == null) { return false; }
            if(o1 is IList && o2 is IList)
            {
                var l1 = (IList)o1;
                var l2 = (IList)o2;
                if (l1.Count != l2.Count) { return false; }
                for(int i = 0; i < l1.Count; i++)
                {
                    if (!l1[i].Equals(l2[i])) { return false; }
                }
                return true;
            }
            return o1.Equals(o2);
        }

        public static bool AreEqual(byte[] o1, byte[] o2)
        {
            if (o1 == null && o2 == null) { return true; }
            if (o1 == null || o2 == null) { return false; }
            if (o1.Length != o2.Length) { return false; }
            for (int i = 0; i < o1.Length; i++)
            {
                if (o1[i] != o2[i]) { return false; }
            }
            return true;
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
            Console.WriteLine(Usage);
            return 1;
        }

        public static void CatchAll(CallbackVoidHandler callback)
        {
            IfCatchException(true, callback);
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
            if (t == typeof(long))
            {
                return 0L;
            }
            if (IncludeDateTime && t == typeof(DateTime))
            {
                return DateTime.MinValue;
            }
            if (IncludeDateTime && t == typeof(Date))
            {
                return new Date(DateTime.MinValue);
            }
            if (IncludeDateTime && t == typeof(Time))
            {
                return new Time(DateTime.MinValue);
            }
            if (t == typeof(Guid))
            {
                return Guid.Empty;
            }
            throw new NotSupportedException(ExceptionText);
        }

        public static List<T> NewList<T>(params T[] ts)
        {
            var ret = new List<T>();
            foreach (T t in ts)
            {
                ret.Add(t);
            }
            return ret;
        }
    }
}
