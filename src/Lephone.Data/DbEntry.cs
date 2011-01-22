using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Lephone.Core;
using Lephone.Data.QuerySyntax;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;

namespace Lephone.Data
{
    public static class DbEntry
    {
        public const string CountColumn = "it__count__";

        #region NewTransaction

        public static readonly DataProvider Provider = new DataProvider("");

        public static void UsingTransaction(CallbackVoidHandler callback)
        {
            Provider.UsingTransaction(callback);
        }

        public static void UsingTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            Provider.UsingTransaction(il, callback);
        }

        public static void NewTransaction(CallbackVoidHandler callback)
        {
            Provider.NewTransaction(callback);
        }

        public static void NewTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            Provider.NewTransaction(il, callback);
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

        public static void Save(object obj)
        {
            GetOperator(obj.GetType()).Save(obj);
        }

        public static void Save(params object[] objs)
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

        public static void Update(object obj)
		{
            GetOperator(obj.GetType()).Update(obj);
		}

        public static void Insert(object obj)
		{
            GetOperator(obj.GetType()).Insert(obj);
		}

        public static int Delete(object obj)
		{
            return GetOperator(obj.GetType()).Delete(obj);
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

        public static DbObjectList<GroupByObject<T1>> GetGroupBy<T1>(Type dbObjectType, Condition iwc, OrderBy order, string columnName)
        {
            return GetOperator(dbObjectType).GetGroupBy<T1>(iwc, order, columnName);
        }

        public static DbObjectList<GroupBySumObject<T1, T2>> GetGroupBySum<T1, T2>(Type dbObjectType, Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            return GetOperator(dbObjectType).GetGroupBySum<T1, T2>(iwc, order, groupbyColumnName, sumColumnName);
        }

        public static DbObjectList<T> ExecuteList<T>(string sqlStr, params object[] os) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).ExecuteList<T>(sqlStr, os);
        }

        public static DbObjectList<T> ExecuteList<T>(SqlStatement sql) where T : class, IDbObject
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

        public static int Delete<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject
        {
            return GetOperator(typeof(T)).Delete(expr);
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

        #endregion
    }
}
