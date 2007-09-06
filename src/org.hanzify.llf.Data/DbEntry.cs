
#region usings

using System;
using System.Data;
using System.Collections;
using org.hanzify.llf.util;
using org.hanzify.llf.util.Logging;
using org.hanzify.llf.Data.QuerySyntax;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Builder;

#endregion

namespace org.hanzify.llf.Data
{
    public static class DbEntry
    {
        public const string CountColumn = "it__count__";

        #region UsingTransaction

        public static void UsingExistedTransaction(CallbackVoidHandler callback)
        {
            _Instance.UsingExistedTransaction(callback);
        }

        public static void UsingExistedTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            _Instance.UsingExistedTransaction(il, callback);
        }

        public static void UsingTransaction(CallbackVoidHandler callback)
        {
            _Instance.UsingTransaction(callback);
        }

        public static void UsingTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            _Instance.UsingTransaction(il, callback);
        }

        #endregion

        #region Instance

        private static readonly DbContext _Instance = new DbContext(EntryConfig.Default);

        public static DbContext Context
        {
            get { return _Instance; }
        }

        #endregion

        #region Shortcut

        public static IWhere<T> From<T>()
        {
            return _Instance.From<T>();
        }

        public static T GetObject<T>(object key)
        {
            return _Instance.GetObject<T>(key);
        }

        public static T GetObject<T>(WhereCondition c)
        {
            return _Instance.GetObject<T>(c);
        }

        public static T GetObject<T>(WhereCondition c, OrderBy ob)
        {
            return _Instance.GetObject<T>(c, ob);
        }

        public static void Save(object obj)
        {
            _Instance.Save(obj);
        }

        public static void Update(object obj)
		{
            _Instance.Update(obj);
		}

        public static void Insert(object obj)
		{
            _Instance.Insert(obj);
		}

        public static void Delete(object obj)
		{
            _Instance.Delete(obj);
		}

        public static void Delete<T>(WhereCondition iwc)
        {
            _Instance.Delete<T>(iwc);
        }

        #endregion
    }
}
