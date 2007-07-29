
#region usings

using System;
using System.Collections;

#endregion

namespace org.hanzify.llf.util
{
    public delegate void CallbackVoidHandler();
    public delegate void CallbackObjectHandler<T>(T o);
    public delegate TR CallbackHandler<T, TR>(T o);

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
    }
}
