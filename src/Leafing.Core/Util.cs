using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leafing.Core.Ioc;

namespace Leafing.Core
{
    public static class Util
    {
        public const string Version = "4.2";
        public const string Copyright = "Copyright © Leafing Studio 2012";
        public const string Product = "Leafing Framework";
        public const string Company = "Leafing Studio";
        public const string Trademark = "http://dbentry.codeplex.com";

        #region Assersion

        public static void Assert(bool assertCondition)
        {
            Assert(assertCondition, "");
        }

        public static void Assert(bool assertCondition, string failedMessage, params object[] os)
        {
            if(!assertCondition)
            {
                string s = string.Format(failedMessage, os);
                throw new LeafingException(s);
            }
        }

        public static bool AreEqual(object o1, object o2)
        {
            if(o1 == null && o2 == null)
            {
                return true;
            }
            if(o1 == null || o2 == null)
            {
                return false;
            }
            if(o1 is IList && o2 is IList)
            {
                var l1 = (IList)o1;
                var l2 = (IList)o2;
                if(l1.Count != l2.Count)
                {
                    return false;
                }
                for(int i = 0; i < l1.Count; i++)
                {
                    if(!l1[i].Equals(l2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return o1.Equals(o2);
        }

        public static bool AreEqual(byte[] o1, byte[] o2)
        {
            if(o1 == null && o2 == null)
            {
                return true;
            }
            if(o1 == null || o2 == null)
            {
                return false;
            }
            if(o1.Length != o2.Length)
            {
                return false;
            }
            for(int i = 0; i < o1.Length; i++)
            {
                if(o1[i] != o2[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        public static int Main(string[] args, int minArgCount, string usage, Action callback)
        {
            if(args.Length >= minArgCount)
            {
                try
                {
                    callback();
                    return 0;
                }
                catch(Exception ex)
                {
                    if(args.Length > minArgCount && args[args.Length - 1] == "-m")
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
            Console.WriteLine(usage);
            return 1;
        }

        public static void CatchAll(Action callback)
        {
            IfCatchException(true, callback);
        }

        public static void IfCatchException(bool catchException, Action callback)
        {
            // ReSharper disable EmptyGeneralCatchClause
            if(catchException)
            {
                try
                {
                    callback();
                }
                catch
                {
                }
            }
            else
            {
                callback();
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        public static void TryEnumerate(object obj, Action<object> callback)
        {
            if(obj == null)
            {
                return;
            }
            if(obj is IEnumerable)
            {
                foreach(object o in (IEnumerable)obj)
                {
                    callback(o);
                }
            }
            else
            {
                callback(obj);
            }
        }

        public static object GetEmptyValue(Type t)
        {
            return GetEmptyValue(t, true, "Unknown Type");
        }

        public static object GetEmptyValue(Type t, bool includeDateTime, string exceptionText)
        {
            if(t == typeof(int))
            {
                return 0;
            }
            if(t == typeof(long))
            {
                return 0L;
            }
            if(includeDateTime && t == typeof(DateTime))
            {
                return DateTime.MinValue;
            }
            if(includeDateTime && t == typeof(Date))
            {
                return new Date(DateTime.MinValue);
            }
            if(includeDateTime && t == typeof(Time))
            {
                return new Time(DateTime.MinValue);
            }
            if(t == typeof(Guid))
            {
                return Guid.Empty;
            }
            throw new NotSupportedException(exceptionText);
        }

        public static List<T> NewList<T>(params T[] ts)
        {
            return ts.ToList();
        }

        #region Encoding

        public static Encoding GetGbkEncoding()
        {
            return Encoding.GetEncoding(936);
        }

        public static Encoding GetBig5Encoding()
        {
            return Encoding.GetEncoding(950);
        }

        public static Encoding GetShiftJisEncoding()
        {
            return Encoding.GetEncoding(932);
        }

        public static Encoding GetKoreaEncoding()
        {
            return Encoding.GetEncoding(949);
        }

        #endregion

        #region Misc Shortcut

        private static class MiscHandler
        {
            public static readonly MiscProvider Instence = SimpleContainer.Get<MiscProvider>();
        }

        public static DateTime Now
        {
            get { return MiscHandler.Instence.Now; }
        }

        public static Guid NewGuid()
        {
            return MiscHandler.Instence.NewGuid();
        }

        public static long Secends
        {
            get { return MiscHandler.Instence.Secends; }
        }

        public static int SystemRunningMillisecends
        {
            get { return MiscHandler.Instence.SystemRunningMillisecends; }
        }

        public static void Sleep(int millisecends)
        {
            MiscHandler.Instence.Sleep(millisecends);
        }

        #endregion
    }
}
