using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Lephone.Util;
using Lephone.Data.Definition;
using Lephone.Data.QuerySyntax;
using Lephone.Data.Common;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Caching;

namespace Lephone.Data
{
    public class DbContext : DataProvider
    {
        private Dictionary<string, int> _tableNames;
        // for remoting only
        public DbContext() : this(EntryConfig.Default) { }

        public DbContext(string prefix) : this(EntryConfig.GetDriver(prefix)) { }

        public DbContext(DbDriver driver) : base(driver) { }

        public DateTime GetDatabaseTime()
        {
            string sqlstr = "select " + Dialect.DbNowString;
            DateTime dt = Convert.ToDateTime(ExecuteScalar(sqlstr));
            return dt;
        }

        private void TryCreateTable(Type dbObjectType)
        {
            if (Driver.AutoCreateTable)
            {
                if (_tableNames == null)
                {
                    InitTableNames();
                }
                Debug.Assert(_tableNames != null);
                ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
                if(oi.CreateTables != null)
                {
                    foreach(var type in oi.CreateTables)
                    {
                        ObjectInfo oi1 = ObjectInfo.GetInstance(type);
                        InnerTryCreateTable(type, oi1);
                    }
                }
                else
                {
                    InnerTryCreateTable(dbObjectType, oi);
                }
            }
        }

        private void InnerTryCreateTable(Type dbObjectType, ObjectInfo oi)
        {
            string name = oi.From.GetMainTableName();
            if (name != null && !_tableNames.ContainsKey(name.ToLower()))
            {
                IfUsingTransaction(Dialect.NeedCommitCreateFirst, delegate
                {
                    Create(dbObjectType);
                    if (!string.IsNullOrEmpty(oi.DeleteToTableName))
                    {
                        CreateDeleteToTable(dbObjectType);
                    }
                    foreach (CrossTable mt in oi.CrossTables.Values)
                    {
                        if (!_tableNames.ContainsKey(mt.Name.ToLower()))
                        {
                            Debug.Assert(dbObjectType.Assembly.FullName != null);
                            if (dbObjectType.Assembly.FullName.StartsWith(MemoryAssembly.DefaultAssemblyName))
                            {
                                CreateCrossTable(dbObjectType.BaseType, mt.HandleType);
                            }
                            else
                            {
                                CreateCrossTable(dbObjectType, mt.HandleType);
                            }
                        }
                    }
                });
            }
        }

        protected Stack<List<string>> TransLists = new Stack<List<string>>();

        protected internal override void OnBeginTransaction()
        {
            if (DataSetting.CacheEnabled && !DataSetting.CacheClearWhenError)
            {
                TransLists.Push(new List<string>());
            }
        }

        protected internal override void OnCommittedTransaction()
        {
            if (DataSetting.CacheEnabled && !DataSetting.CacheClearWhenError)
            {
                TransLists.Pop();
            }
        }

        protected internal override void OnTransactionError()
        {
            if (DataSetting.CacheEnabled)
            {
                if (DataSetting.CacheClearWhenError)
                {
                    CacheProvider.Instance.Clear();
                }
                else
                {
                    List<string> keyList = TransLists.Pop();
                    foreach (string key in keyList)
                    {
                        CacheProvider.Instance.Remove(key);
                    }
                }
            }
        }

        protected virtual void SetCachedObject(object obj)
        {
            string key = KeyGenerator.Instance[obj];
            CacheProvider.Instance[key] = ObjectInfo.CloneObject(obj);
            if (TransLists.Count > 0)
            {
                TransLists.Peek().Add(key);
            }
        }

        private void IfUsingTransaction(bool isUsing, CallbackVoidHandler callback)
        {
            if (isUsing)
            {
                NewTransaction(callback);
            }
            else
            {
                callback();
            }
        }

        private void InitTableNames()
        {
            _tableNames = new Dictionary<string, int>();
            foreach (string s in GetTableNames())
            {
                _tableNames.Add(s.ToLower(), 1);
            }
        }

        public IWhere<T> From<T>() where T : class, IDbObject
        {
            return new QueryContent<T>(this);
        }

        public long GetResultCount(Type dbObjectType, Condition iwc)
        {
            return GetResultCount(dbObjectType, iwc, false);
        }

        public long GetResultCount(Type dbObjectType, Condition iwc, bool isDistinct)
        {
            TryCreateTable(dbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetResultCountStatement(Dialect, iwc, isDistinct);
            oi.LogSql(sql);
            object ro = ExecuteScalar(sql);
            return Convert.ToInt64(ro);
        }

        public decimal? GetMax(Type dbObjectType, Condition iwc, string columnName)
        {
            object o = GetMaxObject(dbObjectType, iwc, columnName);
            if (o == null) return null;
            return Convert.ToDecimal(o);
        }

        public DateTime? GetMaxDate(Type dbObjectType, Condition iwc, string columnName)
        {
            object o = GetMaxObject(dbObjectType, iwc, columnName);
            if (o == null) return null;
            return Convert.ToDateTime(o);
        }

        public object GetMaxObject(Type dbObjectType, Condition iwc, string columnName)
        {
            TryCreateTable(dbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetMaxStatement(Dialect, iwc, columnName);
            oi.LogSql(sql);
            object ro = ExecuteScalar(sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return ro;
        }

        public decimal? GetMin(Type dbObjectType, Condition iwc, string columnName)
        {
            object o = GetMinObject(dbObjectType, iwc, columnName);
            if (o == null) return null;
            return Convert.ToDecimal(o);
        }

        public DateTime? GetMinDate(Type dbObjectType, Condition iwc, string columnName)
        {
            object o = GetMinObject(dbObjectType, iwc, columnName);
            if (o == null) return null;
            return Convert.ToDateTime(o);
        }

        public object GetMinObject(Type dbObjectType, Condition iwc, string columnName)
        {
            TryCreateTable(dbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetMinStatement(Dialect, iwc, columnName);
            oi.LogSql(sql);
            object ro = ExecuteScalar(sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return ro;
        }

        public decimal? GetSum(Type dbObjectType, Condition iwc, string columnName)
        {
            TryCreateTable(dbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetSumStatement(Dialect, iwc, columnName);
            oi.LogSql(sql);
            object ro = ExecuteScalar(sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(ro);
        }

        public DbObjectList<GroupByObject<T1>> GetGroupBy<T1>(Type dbObjectType, Condition iwc, OrderBy order, string columnName)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetGroupByStatement(Dialect, iwc, order, columnName);
            oi.LogSql(sql);
            var list = new DbObjectList<GroupByObject<T1>>();
            IProcessor ip = GetListProcessor(list, dbObjectType);
            DataLoadDirect(ip, typeof(GroupByObject<T1>), dbObjectType, sql, true);
            return list;
        }

        public DbObjectList<GroupBySumObject<T1, T2>> GetGroupBySum<T1, T2>(Type dbObjectType, Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetGroupBySumStatement(Dialect, iwc, order, groupbyColumnName, sumColumnName);
            oi.LogSql(sql);
            var list = new DbObjectList<GroupBySumObject<T1, T2>>();
            IProcessor ip = GetListProcessor(list, dbObjectType);
            DataLoadDirect(ip, typeof(GroupBySumObject<T1, T2>), dbObjectType, sql, true);
            return list;
        }

        public DbObjectList<T> ExecuteList<T>(string sqlStr) where T : class, IDbObject
        {
            return ExecuteList<T>(new SqlStatement(sqlStr));
        }

        public DbObjectList<T> ExecuteList<T>(string sqlStr, params object[] os) where T : class, IDbObject
        {
            return ExecuteList<T>(GetSqlStatement(sqlStr, os));
        }

        public DbObjectList<T> ExecuteList<T>(SqlStatement sql) where T : class, IDbObject
        {
            var ret = new DbObjectList<T>();
            FillCollection(ret, typeof(T), sql);
            return ret;
        }

        protected static IProcessor GetListProcessor(IList il, Type t)
        {
            if (DataSetting.CacheAnySelectedItem)
            {
                return new CachedListInserter(il, t);
            }
            if (DataSetting.MaxRecords == 0)
            {
                return new ListInserter(il);
            }
            return new LimitedListInserter(il);
        }

        public void FillCollection(IList list, Type dbObjectType, SqlStatement sql)
        {
            IProcessor ip = GetListProcessor(list, dbObjectType);
            DataLoad(ip, dbObjectType, sql);
        }

        public void FillCollection(IList list, Type dbObjectType, Condition iwc, OrderBy oc, Range lc)
        {
            FillCollection(list, dbObjectType, iwc, oc, lc, false);
        }

        public void FillCollection(IList list, Type dbObjectType, Condition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            IProcessor ip = GetListProcessor(list, dbObjectType);
            DataLoad(ip, dbObjectType, null, iwc, oc, lc, isDistinct);
        }

        public void FillCollection(IList list, Type dbObjectType, FromClause from, Condition iwc, OrderBy oc, Range lc)
        {
            FillCollection(list, dbObjectType, from, iwc, oc, lc, false);
        }

        public void FillCollection(IList list, Type dbObjectType, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            IProcessor ip = GetListProcessor(list, dbObjectType);
            DataLoad(ip, dbObjectType, from, iwc, oc, lc, isDistinct);
        }

        public void DataLoad(IProcessor ip, Type dbObjectType, SqlStatement sql)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            oi.LogSql(sql);
            DataLoadDirect(ip, dbObjectType, sql, false);
        }

        public void DataLoad(IProcessor ip, Type dbObjectType, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetSelectStatement(Dialect, from, iwc, oc, lc, isDistinct);
            oi.LogSql(sql);
            DataLoadDirect(ip, dbObjectType, sql);
        }

        private void DataLoadDirect(IProcessor ip, Type dbObjectType, SqlStatement sql)
        {
            DataLoadDirect(ip, dbObjectType, sql, true);
        }

        private void DataLoadDirect(IProcessor ip, Type dbObjectType, SqlStatement sql, bool useIndex)
        {
            DataLoadDirect(ip, dbObjectType, dbObjectType, sql, useIndex);
        }

        private void DataLoadDirect(IProcessor ip, Type returnType, Type dbObjectType, SqlStatement sql, bool useIndex)
        {
            TryCreateTable(dbObjectType);
            int startIndex = sql.StartIndex;
            int endIndex = sql.EndIndex;
            if (Dialect.SupportsRangeStartIndex && endIndex > 0)
            {
                endIndex = endIndex - startIndex + 1;
                startIndex = 1;
            }
            ExecuteDataReader(sql, returnType, delegate(IDataReader dr)
            {
                int count = 0;
                while (dr.Read())
                {
                    count++;
                    if (count >= startIndex)
                    {
                        object di = ObjectInfo.CreateObject(this, returnType, dr, useIndex);
                        if (!ip.Process(di))
                        {
                            break;
                        }
                    }
                    if (endIndex > 0 && count >= endIndex)
                    {
                        break;
                    }
                }
            });
        }

        public T GetObject<T>(object key) where T : class, IDbObject
        {
            return (T)GetObject(typeof(T), key);
        }

        public T GetObject<T>(Condition c) where T : class, IDbObject
        {
            return (T)GetObject(typeof(T), c, null, null);
        }

        public T GetObject<T>(Condition c, OrderBy ob) where T : class, IDbObject
        {
            return (T)GetObject(typeof(T), c, ob);
        }

        public object GetObject(Type t, object key)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            if (oi.HasOnePremarykey)
            {
                if (DataSetting.CacheEnabled && oi.Cacheable)
                {
                    object co = CacheProvider.Instance[KeyGenerator.Instance.GetKey(t, key)];
                    if (co != null)
                    {
                        object objInCache = ObjectInfo.CloneObject(co, this);
                        return objInCache;
                    }
                }

                string keyname = oi.KeyFields[0].Name;
                object obj = GetObject(t, CK.K[keyname] == key, null, null);

                if (DataSetting.CacheEnabled && oi.Cacheable)
                {
                    if (obj != null)
                    {
                        SetCachedObject(obj);
                    }
                }
                return obj;
            }
            throw new DataException("To call this function, the table must have one premary key.");
        }

        public object GetObject(Type t, Condition c, OrderBy ob)
        {
            return GetObject(t, c, ob, (ob == null) ? null : new Range(1, 1));
        }

        internal object GetObject(Type t, Condition c, OrderBy ob, Range r)
        {
            IList il = new ArrayList();
            FillCollection(il, t, c, ob, r);
            if (il.Count < 1)
            {
                return null;
            }
            return il[0];
        }

        public void Save(object obj)
        {
            CommonHelper.TryEnumerate(obj, InnerSave);
        }

        private void InnerSave(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            if (!oi.HasOnePremarykey)
            {
                throw new DataException("To call this function, the table must have one primary key.");
            }
            MemberHandler k = oi.KeyFields[0];
            if (oi.HasSystemKey)
            {
                if (k.UnsavedValue == null)
                {
                    throw new DataException("To call this functionn, the UnsavedValue must be set.");
                }
                InnerSave(k.UnsavedValue.Equals(k.GetValue(obj)), obj);
            }
            else
            {
                InnerSave(null == GetObject(obj.GetType(), k.GetValue(obj)), obj);
            }
        }

        private void InnerSave(bool isInsert, object obj)
        {
            if (isInsert)
            {
                Insert(obj);
            }
            else
            {
                Update(obj);
            }
        }

        private void ProcessRelation(ObjectInfo oi, bool parentFirst, object obj,
            CallbackObjectHandler<DataProvider> e1, CallbackObjectHandler<object> e2)
        {
            if (oi.HasAssociate)
            {
                UsingTransaction(delegate
                {
                    if (parentFirst) { e1(this); }
                    object mkey = oi.Handler.GetKeyValue(obj);
                    using (new Scope<object>(mkey))
                    {
                        foreach (MemberHandler f in oi.RelationFields)
                        {
                            var ho = (ILazyLoading)f.GetValue(obj);
                            ho.IsLoaded = true;
                            if (f.IsHasOne || f.IsHasMany || (f.IsHasAndBelongsToMany && parentFirst))
                            {
                                object llo = ho.Read();
                                if (llo == null)
                                {
                                    if (f.IsHasOne)
                                    {
                                        var ho1 = (IHasOne)ho;
                                        if (ho1.LastValue != null)
                                        {
                                            Save(ho1.LastValue);
                                        }
                                    }
                                }
                                else
                                {
                                    if (f.IsHasMany)
                                    {
                                        var ho2 = (IHasMany)ho;
                                        foreach (object item in ho2.RemovedValues)
                                        {
                                            Save(item);
                                        }
                                    }

                                    CommonHelper.TryEnumerate(llo, e2);
                                }
                            }
                            if (f.IsHasAndBelongsToMany)
                            {
                                var so = (IHasAndBelongsToManyRelations)ho;
                                foreach (object n in so.SavedNewRelations)
                                {
                                    SetManyToManyRelation(oi, f.FieldType.GetGenericArguments()[0], oi.Handler.GetKeyValue(obj), n);
                                }
                                foreach (object n in so.RemovedRelations)
                                {
                                    RemoveManyToManyRelation(oi, f.FieldType.GetGenericArguments()[0], oi.Handler.GetKeyValue(obj), n);
                                }
                            }
                        }
                        if (!parentFirst) { e1(this); }
                    }
                });
            }
            else
            {
                e1(this);
            }
        }

        public void Update(object obj)
        {
            Update(obj, ObjectInfo.GetKeyWhereClause(obj));
        }

        private void Update(object obj, Condition iwc)
        {
            Type t = obj.GetType();
            TryCreateTable(t);
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            ProcessRelation(oi, true, obj, delegate(DataProvider dp)
            {
                var to = obj as DbObjectSmartUpdate;
                if (to != null && to.m_UpdateColumns != null)
                {
                    if (to.m_UpdateColumns.Count > 0)
                    {
                        InnerUpdate(obj, iwc, oi, dp);
                    }
                }
                else
                {
                    InnerUpdate(obj, iwc, oi, dp);
                }
            }, delegate(object o)
            {
                SetBelongsToForeignKey(obj, o, oi.KeyFields[0].GetValue(obj));
                Save(o);
            });
        }

        private void InnerUpdate(object obj, Condition iwc, ObjectInfo oi, DataProvider dp)
        {
            SqlStatement sql = oi.Composer.GetUpdateStatement(Dialect, obj, iwc);
            oi.LogSql(sql);
            int n = dp.ExecuteNonQuery(sql);
            if (n == 0)
            {
                throw new DataException("Record doesn't exist OR LockVersion doesn't match!");
            }
            if (obj is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)obj).m_UpdateColumns = new Dictionary<string, object>();
            }
            oi.Composer.ProcessAfterSave(obj);

            if (DataSetting.CacheEnabled && oi.Cacheable && oi.HasOnePremarykey)
            {
                SetCachedObject(obj);
            }
        }

        public void Insert(object obj)
        {
            Type t = obj.GetType();
            TryCreateTable(t);
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            InsertStatementBuilder sb = oi.Composer.GetInsertStatementBuilder(obj);
            if (oi.HasSystemKey)
            {
                ProcessRelation(oi, true, obj, delegate(DataProvider dp)
                {
                    object key = dp.Dialect.ExecuteInsert(dp, sb, oi);
                    ObjectInfo.SetKey(obj, key);
                    foreach (Type t2 in oi.CrossTables.Keys)
                    {
                        SetManyToManyRelation(oi, t2, key, Scope<object>.Current);
                    }
                }, delegate(object o)
                {
                    SetBelongsToForeignKey(obj, o, oi.Handler.GetKeyValue(obj));
                    Save(o);
                });
            }
            else
            {
                SqlStatement sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(sql);
                ExecuteNonQuery(sql);
            }
            if (DataSetting.CacheEnabled && oi.Cacheable && oi.HasOnePremarykey)
            {
                SetCachedObject(obj);
            }
        }

        private void SetManyToManyRelation(ObjectInfo oi, Type t, object key1, object key2)
        {
            if (oi.CrossTables.ContainsKey(t) && key1 != null && key2 != null)
            {
                CrossTable mt = oi.CrossTables[t];
                var sb = new InsertStatementBuilder(mt.Name);
                sb.Values.Add(new KeyValue(mt.ColumeName1, key1));
                sb.Values.Add(new KeyValue(mt.ColumeName2, key2));
                SqlStatement sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(sql);
                ExecuteNonQuery(sql);
            }
        }

        private void RemoveManyToManyRelation(ObjectInfo oi, Type t, object key1, object key2)
        {
            if (oi.CrossTables.ContainsKey(t) && key1 != null && key2 != null)
            {
                CrossTable mt = oi.CrossTables[t];
                var sb = new DeleteStatementBuilder(mt.Name);
                Condition c = CK.K[mt.ColumeName1] == key1;
                c &= CK.K[mt.ColumeName2] == key2;
                sb.Where.Conditions = c;
                SqlStatement sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(sql);
                ExecuteNonQuery(sql);
            }
        }

        private static void SetBelongsToForeignKey(object obj, object subobj, object foreignKey)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(subobj.GetType());
            MemberHandler mh = oi.GetBelongsTo(obj.GetType());
            if (mh != null)
            {
                var ho = mh.GetValue(subobj) as IBelongsTo;
                if (ho != null)
                {
                    ho.ForeignKey = foreignKey;
                }
            }
        }

        public int Delete(object obj)
        {
            Type t = obj.GetType();
            TryCreateTable(t);
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            SqlStatement sql = oi.Composer.GetDeleteStatement(Dialect, obj);
            oi.LogSql(sql);
            int ret = 0;
            ProcessRelation(oi, false, obj, delegate(DataProvider dp)
            {
                ret += dp.ExecuteNonQuery(sql);
                if (DataSetting.CacheEnabled && oi.Cacheable)
                {
                    CacheProvider.Instance.Remove(KeyGenerator.Instance[obj]);
                }
                ret += DeleteRelation(oi, obj);
            }, o => Delete(o));
            if (oi.KeyFields[0].UnsavedValue != null)
            {
                oi.KeyFields[0].SetValue(obj, oi.KeyFields[0].UnsavedValue);
            }
            return ret;
        }

        private int DeleteRelation(ObjectInfo oi, object obj)
        {
            int ret = 0;
            foreach (CrossTable mt in oi.CrossTables.Values)
            {
                var sb = new DeleteStatementBuilder(mt.Name);
                sb.Where.Conditions = CK.K[mt.ColumeName1] == oi.Handler.GetKeyValue(obj);
                SqlStatement sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(sql);
                ret += ExecuteNonQuery(sql);
            }
            return ret;
        }

        public int Delete<T>(Condition iwc) where T : class, IDbObject
        {
            Type t = typeof(T);
            TryCreateTable(t);
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            SqlStatement sql = oi.Composer.GetDeleteStatement(Dialect, iwc);
            oi.LogSql(sql);
            return ExecuteNonQuery(sql);
        }

        public void DropTable(Type dbObjectType)
        {
            DropTable(dbObjectType, true);
        }

        public void DropTable(Type dbObjectType, bool catchException)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            string tn = oi.From.GetMainTableName();
            DropTable(tn, catchException, oi);
            if (oi.HasSystemKey)
            {
                CommonHelper.IfCatchException(true, () => Dialect.ExecuteDropSequence(this, tn));
            }
            foreach (CrossTable mt in oi.CrossTables.Values)
            {
                DropTable(mt.Name, catchException, oi);
            }
        }

        private void DropTable(string tableName, bool catchException, ObjectInfo oi)
        {
            string s = "DROP TABLE " + Dialect.QuoteForTableName(tableName);
            var sql = new SqlStatement(s);
            oi.LogSql(sql);
            CommonHelper.IfCatchException(catchException, () => ExecuteNonQuery(sql));
            if (Driver.AutoCreateTable && _tableNames != null)
            {
                _tableNames.Remove(tableName.ToLower());
            }
        }

        public void DropAndCreate(Type dbObjectType)
        {
            DropTable(dbObjectType, true);
            Create(dbObjectType);
        }

        public void Create(Type dbObjectType)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            SqlStatement sql = oi.Composer.GetCreateStatement(Dialect);
            oi.LogSql(sql);
            ExecuteNonQuery(sql);
            if (Driver.AutoCreateTable && _tableNames != null)
            {
                _tableNames.Add(oi.From.GetMainTableName().ToLower(), 1);
            }
        }

        public void CreateDeleteToTable(Type dbObjectType)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(dbObjectType);
            CreateTableStatementBuilder sb = oi.Composer.GetCreateTableStatementBuilder();
            sb.TableName = oi.DeleteToTableName;
            sb.Columns.Add(new ColumnInfo("DeletedOn", typeof(DateTime), false, false, false, false, 0));
            SqlStatement sql = sb.ToSqlStatement(Dialect);
            oi.LogSql(sql);
            ExecuteNonQuery(sql);
            if (Driver.AutoCreateTable && _tableNames != null)
            {
                _tableNames.Add(oi.DeleteToTableName.ToLower(), 1);
            }
        }

        public void CreateCrossTable(Type t1, Type t2)
        {
            ObjectInfo oi1 = ObjectInfo.GetInstance(t1);
            ObjectInfo oi2 = ObjectInfo.GetInstance(t2);
            if (!(oi1.CrossTables.ContainsKey(t2) && oi2.CrossTables.ContainsKey(t1)))
            {
                throw new DataException("They are not many to many relation ship classes!");
            }
            if (oi1.KeyFields.Length <= 0 || oi2.KeyFields.Length <= 0)
            {
                throw new DataException("The relation table must have key column!");
            }
            CrossTable mt1 = oi1.CrossTables[t2];
            var cts = new CreateTableStatementBuilder(mt1.Name);
            var ls = new List<string> { mt1.ColumeName1, mt1.ColumeName2 };
            ls.Sort();
            cts.Columns.Add(new ColumnInfo(ls[0], oi1.KeyFields[0].FieldType, false, false, false, false, 0));
            cts.Columns.Add(new ColumnInfo(ls[1], oi2.KeyFields[0].FieldType, false, false, false, false, 0));
            // add index
            cts.Indexes.Add(new DbIndex(null, false, (ASC)mt1.ColumeName1));
            cts.Indexes.Add(new DbIndex(null, false, (ASC)mt1.ColumeName2));
            // execute
            SqlStatement sql = cts.ToSqlStatement(Dialect);
            oi1.LogSql(sql);
            ExecuteNonQuery(sql);
            if (Driver.AutoCreateTable && _tableNames != null)
            {
                _tableNames.Add(mt1.Name.ToLower(), 1);
            }
        }

        #region Linq methods

        public T GetObject<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject
        {
            var wc = Linq.ExpressionParser<T>.Parse(expr);
            return GetObject<T>(wc);
        }

        public int Delete<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject
        {
            var wc = Linq.ExpressionParser<T>.Parse(expr);
            return Delete<T>(wc);
        }

        #endregion
    }
}
