using System.Data;
using Lephone.Util;
using Lephone.Data.QuerySyntax;
using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.Data
{
    public static class DbEntry
    {
        public const string CountColumn = "it__count__";

        #region NewTransaction

        public static void UsingTransaction(CallbackVoidHandler callback)
        {
            _Instance.UsingTransaction(callback);
        }

        public static void UsingTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            _Instance.UsingTransaction(il, callback);
        }

        public static void NewTransaction(CallbackVoidHandler callback)
        {
            _Instance.NewTransaction(callback);
        }

        public static void NewTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            _Instance.NewTransaction(il, callback);
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

        public static IWhere<T> From<T>() where T : class, IDbObject
        {
            return _Instance.From<T>();
        }

        public static T GetObject<T>(object key) where T : class, IDbObject
        {
            return _Instance.GetObject<T>(key);
        }

        public static T GetObject<T>(Condition c) where T : class, IDbObject
        {
            return _Instance.GetObject<T>(c);
        }

        public static T GetObject<T>(Condition c, OrderBy ob) where T : class, IDbObject
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

        public static int Delete(object obj)
		{
            return _Instance.Delete(obj);
		}

        public static int Delete<T>(Condition iwc) where T : class, IDbObject
        {
            return _Instance.Delete<T>(iwc);
        }

        #endregion
    }
}
