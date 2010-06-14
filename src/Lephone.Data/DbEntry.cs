using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Lephone.Core;
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
            Context.UsingTransaction(callback);
        }

        public static void UsingTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            Context.UsingTransaction(il, callback);
        }

        public static void NewTransaction(CallbackVoidHandler callback)
        {
            Context.NewTransaction(callback);
        }

        public static void NewTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            Context.NewTransaction(il, callback);
        }

        #endregion

        #region Instance

        public static readonly DbContext Context;
        private static readonly HybridDictionary Jar;

        static DbEntry()
	    {
            Jar = new HybridDictionary();
            Context = GetContext(DataSettings.DefaultContext);
	    }

        public static DbContext GetContext(string prefix)
        {
            if (string.IsNullOrEmpty(prefix) && Context != null)
            {
                return Context;
            }
            Debug.Assert(prefix != null);
            if(Jar.Contains(prefix))
            {
                return (DbContext) Jar[prefix];
            }
            lock (Jar.SyncRoot)
            {
                if (Jar.Contains(prefix))
                {
                    return (DbContext)Jar[prefix];
                }
                var ctx = new DbContext(prefix);
                Jar[prefix] = ctx;
                return ctx;
            }
        }

        public static DbContext GetContext(Type type)
        {
            var oi = ObjectInfo.GetInstance(type);
            return oi.Context;
        }

        #endregion

        #region Shortcut

        public static IWhere<T> From<T>() where T : class, IDbObject
        {
            return GetContext(typeof(T)).From<T>();
        }

        public static T GetObject<T>(object key) where T : class, IDbObject
        {
            return GetContext(typeof(T)).GetObject<T>(key);
        }

        public static T GetObject<T>(Condition c) where T : class, IDbObject
        {
            return GetContext(typeof(T)).GetObject<T>(c);
        }

        public static T GetObject<T>(Condition c, OrderBy ob) where T : class, IDbObject
        {
            return GetContext(typeof(T)).GetObject<T>(c, ob);
        }

        public static T GetObject<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject
        {
            return GetContext(typeof(T)).GetObject(expr);
        }

        public static void Save(object obj)
        {
            GetContext(obj.GetType()).Save(obj);
        }

        public static void Update(object obj)
		{
            GetContext(obj.GetType()).Update(obj);
		}

        public static void Insert(object obj)
		{
            GetContext(obj.GetType()).Insert(obj);
		}

        public static int Delete(object obj)
		{
            return GetContext(obj.GetType()).Delete(obj);
		}

        public static int Delete<T>(Condition iwc) where T : class, IDbObject
        {
            return GetContext(typeof(T)).Delete<T>(iwc);
        }

        #endregion

        public static List<Type> GetAllModels(Assembly assembly)
        {
            var idot = typeof(IDbObject);
            var ts = new List<Type>();
            foreach (var t in assembly.GetExportedTypes())
            {
                if (!t.IsGenericType && !t.IsAbstract)
                {
                    foreach (var @interface in t.GetInterfaces())
                    {
                        if (@interface == idot)
                        {
                            ts.Add(t);
                        }
                    }
                }
            }
            ts.Sort((x, y) => x.FullName.CompareTo(y.FullName));
            return ts;
        }
    }
}
