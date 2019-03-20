using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Leafing.Core;
using Leafing.Core.Logging;
using Leafing.Data.Caching;
using Leafing.Data.Model;
using Leafing.Data.Definition;
using Leafing.Data.Model.QuerySyntax;
using Leafing.Data.SqlEntry;
using System.Runtime.CompilerServices;
using Leafing.Data.Model.Handler.Generator;
using Leafing.Core.Setting;

[assembly: InternalsVisibleTo("Leafing.Extra, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ebce81d3bd5481a9f62666c880d1425968d3074786f29f38f5f42ba7d2497ac56456084097085b82f8980304dd9048da30716d8bcfd920a24a4cee2580fd09cecbe40f7eb8e7211e3e3f592f3aba3b38268c99e124525e7a200015e3ee1e061e6f1387ac474577b8023af58c3bbcc790f26b1745b454862ada11213b130097ef")]
[assembly: InternalsVisibleTo("Leafing.Processor, PublicKey=00240000048000009400000006020000002400005253413100040000010001005133516731555730b303a4c6178fb7b48b3d397983001ac56144e10d446cbba2b8ebe83db05050687897b1a5e0093b04d4a0ad3d8d9db36781221132fcbec4cd6fc6fe9fda32a544acb6505d102539f1bf50aafead17155c57482c25242cf3c6976456c15338db8883303a117f88e6cd2ce6c20877c1eae94cd20729b3cecc8d")]
[assembly: InternalsVisibleTo("Leafing.UnitTest, PublicKey=0024000004800000940000000602000000240000525341310004000001000100636bb6433a3676469f9232335b9c9e9a35b158a31ec6dcb2633ca9ba55438ba353775e8913339902eee57f94de541e1fa9cc5c6621637db6d4b723aabb9eb16ef58ef64c13b2fde1fec9e2e4ab51b2649855deee33ef92f54a89ab7b7c7d44ca2a1f61893a7e35b47975241fa8975a968f72f20694283e06ee5b98f65ad89cd0")]
[assembly: InternalsVisibleTo(MemoryAssembly.DefaultAssemblyName + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100ebce81d3bd5481a9f62666c880d1425968d3074786f29f38f5f42ba7d2497ac56456084097085b82f8980304dd9048da30716d8bcfd920a24a4cee2580fd09cecbe40f7eb8e7211e3e3f592f3aba3b38268c99e124525e7a200015e3ee1e061e6f1387ac474577b8023af58c3bbcc790f26b1745b454862ada11213b130097ef")]

namespace Leafing.Data {
    public static class DbEntry {
        public const string CountColumn = "it__count__";

        public static readonly DataProvider Provider = new DataProvider("");

        #region Transaction

        public static void UsingTransaction(Action callback) {
            if (Scope<ConnectionContext>.Current != null) {
                callback();
                return;
            }
            NewTransaction(callback);
        }

        public static void UsingTransaction(IsolationLevel il, Action callback) {
            if (Scope<ConnectionContext>.Current != null) {
                ConnectionContext et = Scope<ConnectionContext>.Current;
                if (et.IsolationLevel == il) {
                    callback();
                    return;
                }
            }
            NewTransaction(il, callback);
        }

        public static void NewTransaction(Action callback) {
            NewTransaction(IsolationLevel.ReadCommitted, callback);
        }

        public static void NewTransaction(IsolationLevel il, Action callback) {
            NewConnection(delegate {
                var cc = Scope<ConnectionContext>.Current;
                cc.BeginTransaction(il);
                try {
                    callback();
                    cc.Commit();
                    OnTransactionCommitted();
                } catch {
                    try {
                        cc.Rollback();
                    } catch (Exception ex) {
                        Logger.SQL.Error(ex);
                    }
                    throw;
                }
            });
        }

        private static void OnTransactionCommitted() {
            if (ConfigReader.Config.Database.Cache.Enabled) {
                if (Scope<ConnectionContext>.Current.Jar != null && Scope<ConnectionContext>.Current.Jar.Count > 0) {
                    foreach (var obj in Scope<ConnectionContext>.Current.Jar.Values) {
                        string key = KeyGenerator.Instance[obj];
                        CacheProvider.Instance[key] = ModelContext.CloneObject(obj);
                    }
                }
                Scope<ConnectionContext>.Current.Jar = null;
            }
        }

        public static void NewConnection(Action callback) {
            using (var cc = new ConnectionContext()) {
                using (new Scope<ConnectionContext>(cc)) {
                    callback();
                }
            }
        }

        public static void UsingConnection(Action callback) {
            if (Scope<ConnectionContext>.Current != null) {
                callback();
                return;
            }
            NewConnection(callback);
        }

        #endregion

        #region Shortcut

        private static ModelOperator GetOperator(Type type) {
            var ctx = ModelContext.GetInstance(type);
            return ctx.Operator;
        }

        public static IWhere<T> From<T>() where T : class, IDbObject, new() {
            return ModelContext.GetInstance(typeof(T)).From<T>();
        }

        public static T GetObject<T>(object key) where T : class, IDbObject {
            return GetOperator(typeof(T)).GetObject<T>(key);
        }

        public static T GetObject<T>(Condition c) where T : class, IDbObject {
            return GetOperator(typeof(T)).GetObject<T>(c);
        }

        public static T GetObject<T>(Condition c, OrderBy ob) where T : class, IDbObject {
            return GetOperator(typeof(T)).GetObject<T>(c, ob);
        }

        public static T GetObject<T>(ConditionBuilder<T> c) where T : class, IDbObject, new() {
            return GetOperator(typeof(T)).GetObject<T>(c.ToCondition());
        }

        public static T GetObject<T>(ConditionBuilder<T> c, OrderBy ob) where T : class, IDbObject, new() {
            return GetOperator(typeof(T)).GetObject<T>(c.ToCondition(), ob);
        }

        public static T GetObject<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject {
            return GetOperator(typeof(T)).GetObject(expr);
        }

        public static void Save(IDbObject obj) {
            GetOperator(obj.GetType()).Save(obj);
        }

        public static void Save(params IDbObject[] objs) {
            if (objs != null && objs.Length > 0) {
                UsingTransaction(
                    () => {
                        foreach (var o in objs) {
                            Save(o);
                        }
                    });
            }
        }

        public static void Update(IDbObject obj) {
            GetOperator(obj.GetType()).Update(obj);
        }

        public static void Insert(IDbObject obj) {
            GetOperator(obj.GetType()).Insert(obj);
        }

        public static int Delete(IDbObject obj) {
            return GetOperator(obj.GetType()).Delete(obj);
        }

        public static int UpdateBy<T>(Condition condition, object obj) where T : class, IDbObject {
            return GetOperator(typeof(T)).UpdateBy(condition, obj);
        }

        public static int UpdateBy<T>(ConditionBuilder<T> condition, object obj) where T : class, IDbObject, new() {
            return GetOperator(typeof(T)).UpdateBy(condition.ToCondition(), obj);
        }

        public static int UpdateBy<T>(Expression<Func<T, bool>> condition, object obj) where T : class, IDbObject {
            return GetOperator(typeof(T)).UpdateBy(condition, obj);
        }

        public static int DeleteBy<T>(Condition condition) where T : class, IDbObject {
            return GetOperator(typeof(T)).DeleteBy(condition);
        }

        public static int DeleteBy<T>(ConditionBuilder<T> condition) where T : class, IDbObject, new() {
            return GetOperator(typeof(T)).DeleteBy(condition.ToCondition());
        }

        public static int DeleteBy<T>(Expression<Func<T, bool>> condition) where T : class, IDbObject {
            return GetOperator(typeof(T)).DeleteBy(condition);
        }

        public static long GetResultCount(Type dbObjectType, Condition iwc) {
            return GetOperator(dbObjectType).GetResultCount(iwc);
        }

        public static long GetResultCount(Type dbObjectType, Condition iwc, bool isDistinct) {
            return GetOperator(dbObjectType).GetResultCount(iwc, isDistinct);
        }

        public static decimal? GetMax(Type dbObjectType, Condition iwc, string columnName) {
            return GetOperator(dbObjectType).GetMax(iwc, columnName);
        }

        public static DateTime? GetMaxDate(Type dbObjectType, Condition iwc, string columnName) {
            return GetOperator(dbObjectType).GetMaxDate(iwc, columnName);
        }

        public static object GetMaxObject(Type dbObjectType, Condition iwc, string columnName) {
            return GetOperator(dbObjectType).GetMaxObject(iwc, columnName);
        }

        public static decimal? GetMin(Type dbObjectType, Condition iwc, string columnName) {
            return GetOperator(dbObjectType).GetMin(iwc, columnName);
        }

        public static DateTime? GetMinDate(Type dbObjectType, Condition iwc, string columnName) {
            return GetOperator(dbObjectType).GetMinDate(iwc, columnName);
        }

        public static object GetMinObject(Type dbObjectType, Condition iwc, string columnName) {
            return GetOperator(dbObjectType).GetMinObject(iwc, columnName);
        }

        public static decimal? GetSum(Type dbObjectType, Condition iwc, string columnName) {
            return GetOperator(dbObjectType).GetSum(iwc, columnName);
        }

        public static List<GroupByObject<T1>> GetGroupBy<T1>(Type dbObjectType, Condition iwc, OrderBy order, string columnName) {
            return GetOperator(dbObjectType).GetGroupBy<T1>(iwc, order, columnName);
        }

        public static List<GroupBySumObject<T1, T2>> GetGroupBySum<T1, T2>(Type dbObjectType, Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName) {
            return GetOperator(dbObjectType).GetGroupBySum<T1, T2>(iwc, order, groupbyColumnName, sumColumnName);
        }

        public static List<T> ExecuteList<T>(string sqlStr, params object[] os) where T : class, IDbObject {
            return GetOperator(typeof(T)).ExecuteList<T>(sqlStr, os);
        }

        public static List<T> ExecuteList<T>(SqlStatement sql) where T : class, IDbObject {
            return GetOperator(typeof(T)).ExecuteList<T>(sql);
        }

        public static void RebuildTables(Assembly assembly) {
            var list = GetAllModels(assembly);
            foreach (var type in list) {
                var ctx = ModelContext.GetInstance(type);
                if (!string.IsNullOrEmpty(ctx.Info.From.MainTableName)) {
                    ctx.Operator.CreateTableAndRelations(ctx, true);
                }
            }
        }

        public static void DropTable(Type dbObjectType, bool catchException) {
            GetOperator(dbObjectType).DropTable(catchException);
        }

        public static void DropAndCreate(Type dbObjectType) {
            GetOperator(dbObjectType).DropAndCreate();
        }

        public static void Create(Type dbObjectType) {
            GetOperator(dbObjectType).Create();
        }

        public static void CreateCrossTable(Type t1, Type t2) {
            GetOperator(t1).CreateCrossTable(t2);
        }

        public static void CreateDeleteToTable(Type type) {
            GetOperator(type).CreateDeleteToTable();
        }

        public static List<Type> GetAllModels(Assembly assembly) {
            var idot = typeof(IDbObject);
            var ts = new List<Type>();
            foreach (var t in assembly.GetExportedTypes()) {
                if (!t.IsInterface && !t.IsGenericType && !t.IsAbstract) {
                    foreach (var @interface in t.GetInterfaces()) {
                        if (@interface == idot) {
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
