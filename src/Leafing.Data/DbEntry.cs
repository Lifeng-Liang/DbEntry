using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Leafing.Core;
using Leafing.Core.Logging;
using Leafing.Data.Caching;
using Leafing.Data.Model;
using Leafing.Data.Common;
using Leafing.Data.Definition;
using Leafing.Data.Model.QuerySyntax;
using Leafing.Data.SqlEntry;

namespace Leafing.Data
{
    public static class DbEntry
    {
        public const string CountColumn = "it__count__";

        public static readonly DataProvider Provider = new DataProvider("");

        #region Transaction

        public static void UsingTransaction(Action callback)
        {
            if (Scope<ConnectionContext>.Current != null)
            {
                callback();
                return;
            }
            NewTransaction(callback);
        }

        public static void UsingTransaction(IsolationLevel il, Action callback)
        {
            if (Scope<ConnectionContext>.Current != null)
            {
                ConnectionContext et = Scope<ConnectionContext>.Current;
                if (et.IsolationLevel == il)
                {
                    callback();
                    return;
                }
            }
            NewTransaction(callback);
        }

        public static void NewTransaction(Action callback)
        {
            NewTransaction(IsolationLevel.ReadCommitted, callback);
        }

        public static void NewTransaction(IsolationLevel il, Action callback)
        {
            NewConnection(delegate
            {
                var cc = Scope<ConnectionContext>.Current;
                cc.BeginTransaction(il);
                try
                {
                    callback();
                    cc.Commit();
                    OnTransactionCommitted();
                }
                catch
                {
                    try
                    {
                        cc.Rollback();
                    }
                    catch(Exception ex)
                    {
                        Logger.SQL.Error(ex);
                    }
                    throw;
                }
            });
        }

        private static void OnTransactionCommitted()
        {
            if(DataSettings.CacheEnabled)
            {
                if (Scope<ConnectionContext>.Current.Jar != null && Scope<ConnectionContext>.Current.Jar.Count > 0)
                {
                    foreach (var obj in Scope<ConnectionContext>.Current.Jar.Values)
                    {
                        string key = KeyGenerator.Instance[obj];
                        CacheProvider.Instance[key] = ModelContext.CloneObject(obj);
                    }
                }
                Scope<ConnectionContext>.Current.Jar = null;
            }
        }

        public static void NewConnection(Action callback)
        {
            using (var cc = new ConnectionContext())
            {
                using (new Scope<ConnectionContext>(cc))
                {
                    try
                    {
                        callback();
                    }
                    finally
                    {
                        cc.Close();
                    }
                }
            }
        }

        public static void UsingConnection(Action callback)
        {
            if (Scope<ConnectionContext>.Current != null)
            {
                callback();
                return;
            }
            NewConnection(callback);
        }

        #endregion

        #region Shortcut

        private static ModelOperator GetOperator(Type type)
        {
            var ctx = ModelContext.GetInstance(type);
            return ctx.Operator;
        }

        public static IWhere<T> From<T>() where T : class, IDbObject
        {
            return ModelContext.GetInstance(typeof(T)).From<T>();
        }

        public static T GetObject<T>(object key) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).GetObject<T>(key);
        }

        public static T GetObject<T>(Condition c) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).GetObject<T>(c);
        }

        public static T GetObject<T>(Condition c, OrderBy ob) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).GetObject<T>(c, ob);
        }

        public static T GetObject<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).GetObject(expr);
        }

        public static void Save(IDbObject obj)
        {
            GetOperator(obj.GetType()).Save(obj);
        }

        public static void Save(params IDbObject[] objs)
        {
            if(objs != null && objs.Length > 0)
            {
                UsingTransaction(
                    () =>
                        {
                            foreach(var o in objs)
                            {
                                Save(o);
                            }
                        });
            }
        }

        public static void Update(IDbObject obj)
		{
            GetOperator(obj.GetType()).Update(obj);
		}

        public static void Insert(IDbObject obj)
		{
            GetOperator(obj.GetType()).Insert(obj);
		}

        public static int Delete(IDbObject obj)
		{
            return GetOperator(obj.GetType()).Delete(obj);
		}

        public static int UpdateBy<T>(Condition condition, object obj) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).UpdateBy(condition, obj);
        }

        public static int UpdateBy<T>(Expression<Func<T, bool>> condition, object obj) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).UpdateBy(condition, obj);
        }

        public static int DeleteBy<T>(Condition condition) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).DeleteBy(condition);
        }

        public static int DeleteBy<T>(Expression<Func<T, bool>> condition) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).DeleteBy(condition);
        }

        public static long GetResultCount(Type dbObjectType, Condition iwc)
        {
            return GetOperator(dbObjectType).GetResultCount(iwc);
        }

        public static long GetResultCount(Type dbObjectType, Condition iwc, bool isDistinct)
        {
            return GetOperator(dbObjectType).GetResultCount(iwc, isDistinct);
        }

        public static decimal? GetMax(Type dbObjectType, Condition iwc, string columnName)
        {
            return GetOperator(dbObjectType).GetMax(iwc, columnName);
        }

        public static DateTime? GetMaxDate(Type dbObjectType, Condition iwc, string columnName)
        {
            return GetOperator(dbObjectType).GetMaxDate(iwc, columnName);
        }

        public static object GetMaxObject(Type dbObjectType, Condition iwc, string columnName)
        {
            return GetOperator(dbObjectType).GetMaxObject(iwc, columnName);
        }

        public static decimal? GetMin(Type dbObjectType, Condition iwc, string columnName)
        {
            return GetOperator(dbObjectType).GetMin(iwc, columnName);
        }

        public static DateTime? GetMinDate(Type dbObjectType, Condition iwc, string columnName)
        {
            return GetOperator(dbObjectType).GetMinDate(iwc, columnName);
        }

        public static object GetMinObject(Type dbObjectType, Condition iwc, string columnName)
        {
            return GetOperator(dbObjectType).GetMinObject(iwc, columnName);
        }

        public static decimal? GetSum(Type dbObjectType, Condition iwc, string columnName)
        {
            return GetOperator(dbObjectType).GetSum(iwc, columnName);
        }

        public static List<GroupByObject<T1>> GetGroupBy<T1>(Type dbObjectType, Condition iwc, OrderBy order, string columnName)
        {
            return GetOperator(dbObjectType).GetGroupBy<T1>(iwc, order, columnName);
        }

        public static List<GroupBySumObject<T1, T2>> GetGroupBySum<T1, T2>(Type dbObjectType, Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            return GetOperator(dbObjectType).GetGroupBySum<T1, T2>(iwc, order, groupbyColumnName, sumColumnName);
        }

        public static List<T> ExecuteList<T>(string sqlStr, params object[] os) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).ExecuteList<T>(sqlStr, os);
        }

        public static List<T> ExecuteList<T>(SqlStatement sql) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).ExecuteList<T>(sql);
        }

        public static void RebuildTables(Assembly assembly)
        {
            var list = GetAllModels(assembly);
            var ctlist = new List<string>();
            foreach (var type in list)
            {
                var ctx = ModelContext.GetInstance(type);
                if (!string.IsNullOrEmpty(ctx.Info.From.MainTableName))
                {
                    ctx.Operator.DropTable(true);
                    ctx.Operator.CreateTableAndRelations(
                        ctx.Info, ctx.Operator, mt =>
                        {
                            if (ctlist.Contains(mt.Name))
                            {
                                return false;
                            }
                            ctx.Operator.DropTable(mt.Name, true);
                            ctlist.Add(mt.Name);
                            return true;
                        });
                }
            }
        }

        public static void DropTable(Type dbObjectType, bool catchException)
        {
            GetOperator(dbObjectType).DropTable(catchException);
        }

        public static void DropAndCreate(Type dbObjectType)
        {
            GetOperator(dbObjectType).DropAndCreate();
        }

        public static void Create(Type dbObjectType)
        {
            GetOperator(dbObjectType).Create();
        }

        public static void CreateCrossTable(Type t1, Type t2)
        {
            GetOperator(t1).CreateCrossTable(t2);
        }

        public static void CreateDeleteToTable(Type type)
        {
            GetOperator(type).CreateDeleteToTable();
        }

        public static List<Type> GetAllModels(Assembly assembly)
        {
            var idot = typeof(IDbObject);
            var ts = new List<Type>();
            foreach (var t in assembly.GetExportedTypes())
            {
                if (!t.IsInterface && !t.IsGenericType && !t.IsAbstract)
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
            ts.Sort((x, y) => string.Compare(x.FullName, y.FullName));
            return ts;
        }

        #endregion
    }
}
