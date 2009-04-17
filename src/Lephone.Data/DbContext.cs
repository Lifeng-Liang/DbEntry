using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Dictionary<string, int> TableNames;
        // for remoting only
        public DbContext() : this(EntryConfig.Default) { }

        public DbContext(string Prefix) : this(EntryConfig.GetDriver(Prefix)) { }

        public DbContext(DbDriver driver) : base(driver) { }

        public DateTime GetDatabaseTime()
        {
            string sqlstr = "select " + Dialect.DbNowString;
            DateTime dt = Convert.ToDateTime(ExecuteScalar(sqlstr));
            return dt;
        }

        private void TryCreateTable(Type DbObjectType)
        {
            if (Driver.AutoCreateTable)
            {
                if (TableNames == null)
                {
                    InitTableNames();
                }
                ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
                string Name = oi.From.GetMainTableName();
                Debug.Assert(TableNames != null);
                if (!TableNames.ContainsKey(Name.ToLower()))
                {
                    IfUsingTransaction(Dialect.NeedCommitCreateFirst, delegate
                    {
                        Create(DbObjectType);
                        if (!string.IsNullOrEmpty(oi.DeleteToTableName))
                        {
                            CreateDeleteToTable(DbObjectType);
                        }
                        foreach (CrossTable mt in oi.CrossTables.Values)
                        {
                            if (!TableNames.ContainsKey(mt.Name.ToLower()))
                            {
                                Debug.Assert(DbObjectType.Assembly.FullName != null);
                                if (DbObjectType.Assembly.FullName.StartsWith(MemoryAssembly.DefaultAssemblyName))
                                {
                                    CreateCrossTable(DbObjectType.BaseType, mt.HandleType);
                                }
                                else
                                {
                                    CreateCrossTable(DbObjectType, mt.HandleType);
                                }
                            }
                        }
                    });
                }
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
                    List<string> KeyList = TransLists.Pop();
                    foreach (string key in KeyList)
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

        private void IfUsingTransaction(bool IsUsing, CallbackVoidHandler callback)
        {
            if (IsUsing)
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
            TableNames = new Dictionary<string, int>();
            foreach (string s in GetTableNames())
            {
                TableNames.Add(s.ToLower(), 1);
            }
        }

        public IWhere<T> From<T>() where T : class, IDbObject
        {
            return new QueryContent<T>(this);
        }

        public long GetResultCount(Type DbObjectType, WhereCondition iwc)
        {
            TryCreateTable(DbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            SqlStatement Sql = oi.Composer.GetResultCountStatement(Dialect, iwc);
            oi.LogSql(Sql);
            object ro = ExecuteScalar(Sql);
            return Convert.ToInt64(ro);
        }

        public decimal? GetMax(Type DbObjectType, WhereCondition iwc, string columnName)
        {
            TryCreateTable(DbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            SqlStatement Sql = oi.Composer.GetMaxStatement(Dialect, iwc, columnName);
            oi.LogSql(Sql);
            object ro = ExecuteScalar(Sql);
            if(ro == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(ro);
        }

        public decimal? GetMin(Type DbObjectType, WhereCondition iwc, string columnName)
        {
            TryCreateTable(DbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            SqlStatement Sql = oi.Composer.GetMinStatement(Dialect, iwc, columnName);
            oi.LogSql(Sql);
            object ro = ExecuteScalar(Sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(ro);
        }

        public decimal? GetSum(Type DbObjectType, WhereCondition iwc, string columnName)
        {
            TryCreateTable(DbObjectType);
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            SqlStatement Sql = oi.Composer.GetSumStatement(Dialect, iwc, columnName);
            oi.LogSql(Sql);
            object ro = ExecuteScalar(Sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(ro);
        }

        public DbObjectList<GroupByObject<T1>> GetGroupBy<T1>(Type DbObjectType, WhereCondition iwc, OrderBy order, string ColumnName)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            SqlStatement Sql = oi.Composer.GetGroupByStatement(Dialect, iwc, order, ColumnName);
            oi.LogSql(Sql);
            var list = new DbObjectList<GroupByObject<T1>>();
            IProcessor ip = GetListProcessor(list, DbObjectType);
            DataLoadDirect(ip, typeof(GroupByObject<T1>), DbObjectType, Sql, true);
            return list;
        }

        public DbObjectList<T> ExecuteList<T>(string SqlStr) where T : class, IDbObject
        {
            return ExecuteList<T>(new SqlStatement(SqlStr));
        }

        public DbObjectList<T> ExecuteList<T>(string SqlStr, params object[] os) where T : class, IDbObject
        {
            return ExecuteList<T>(GetSqlStatement(SqlStr, os));
        }

        public DbObjectList<T> ExecuteList<T>(SqlStatement Sql) where T : class, IDbObject
        {
            var ret = new DbObjectList<T>();
            FillCollection(ret, typeof(T), Sql);
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

        public void FillCollection(IList list, Type DbObjectType, SqlStatement Sql)
        {
            IProcessor ip = GetListProcessor(list, DbObjectType);
            DataLoad(ip, DbObjectType, Sql);
        }

        public void FillCollection(IList list, Type DbObjectType, WhereCondition iwc, OrderBy oc, Range lc)
        {
            FillCollection(list, DbObjectType, iwc, oc, lc, false);
        }

        public void FillCollection(IList list, Type DbObjectType, WhereCondition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            IProcessor ip = GetListProcessor(list, DbObjectType);
            DataLoad(ip, DbObjectType, null, iwc, oc, lc, isDistinct);
        }

        public void FillCollection(IList list, Type DbObjectType, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            FillCollection(list, DbObjectType, from, iwc, oc, lc, false);
        }

        public void FillCollection(IList list, Type DbObjectType, FromClause from, WhereCondition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            IProcessor ip = GetListProcessor(list, DbObjectType);
            DataLoad(ip, DbObjectType, from, iwc, oc, lc, isDistinct);
        }

        public void DataLoad(IProcessor ip, Type DbObjectType, SqlStatement Sql)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            oi.LogSql(Sql);
            DataLoadDirect(ip, DbObjectType, Sql, false);
        }

        public void DataLoad(IProcessor ip, Type DbObjectType, FromClause from, WhereCondition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            SqlStatement Sql = oi.Composer.GetSelectStatement(Dialect, from, iwc, oc, lc, isDistinct);
            oi.LogSql(Sql);
            DataLoadDirect(ip, DbObjectType, Sql);
        }

        private void DataLoadDirect(IProcessor ip, Type DbObjectType, SqlStatement Sql)
        {
            DataLoadDirect(ip, DbObjectType, Sql, true);
        }

        private void DataLoadDirect(IProcessor ip, Type DbObjectType, SqlStatement Sql, bool UseIndex)
        {
            DataLoadDirect(ip, DbObjectType, DbObjectType, Sql, UseIndex);
        }

        private void DataLoadDirect(IProcessor ip, Type ReturnType, Type DbObjectType, SqlStatement Sql, bool UseIndex)
        {
            TryCreateTable(DbObjectType);
            int StartIndex = Sql.StartIndex;
            int EndIndex = Sql.EndIndex;
            if (Dialect.SupportsRangeStartIndex && EndIndex > 0)
            {
                EndIndex = EndIndex - StartIndex + 1;
                StartIndex = 1;
            }
            ExecuteDataReader(Sql, ReturnType, delegate(IDataReader dr)
            {
                int Count = 0;
                while (dr.Read())
                {
                    Count++;
                    if (Count >= StartIndex)
                    {
                        object di = ObjectInfo.CreateObject(this, ReturnType, dr, UseIndex);
                        if (!ip.Process(di))
                        {
                            break;
                        }
                    }
                    if (EndIndex > 0 && Count >= EndIndex)
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

        public T GetObject<T>(WhereCondition c) where T : class, IDbObject
        {
            return (T)GetObject(typeof(T), c, null, null);
        }

        public T GetObject<T>(WhereCondition c, OrderBy ob) where T : class, IDbObject
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

        public object GetObject(Type t, WhereCondition c, OrderBy ob)
        {
            return GetObject(t, c, ob, (ob == null) ? null : new Range(1, 1));
        }

        internal object GetObject(Type t, WhereCondition c, OrderBy ob, Range r)
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

        private void InnerSave(bool IsInsert, object obj)
        {
            if (IsInsert)
            {
                Insert(obj);
            }
            else
            {
                Update(obj);
            }
        }

        private void ProcessRelation(ObjectInfo oi, bool ParentFirst, object obj,
            CallbackObjectHandler<DataProvider> e1, CallbackObjectHandler<object> e2)
        {
            if (oi.HasAssociate)
            {
                UsingTransaction(delegate
                {
                    if (ParentFirst) { e1(this); }
                    object mkey = oi.Handler.GetKeyValue(obj);
                    using (new Scope<object>(mkey))
                    {
                        foreach (MemberHandler f in oi.RelationFields)
                        {
                            var ho = (ILazyLoading)f.GetValue(obj);
                            ho.IsLoaded = true;
                            if (f.IsHasOne || f.IsHasMany || (f.IsHasAndBelongsToMany && ParentFirst))
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
                        if (!ParentFirst) { e1(this); }
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

        private void Update(object obj, WhereCondition iwc)
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

        private void InnerUpdate(object obj, WhereCondition iwc, ObjectInfo oi, DataProvider dp)
        {
            SqlStatement Sql = oi.Composer.GetUpdateStatement(Dialect, obj, iwc);
            oi.LogSql(Sql);
            int n = dp.ExecuteNonQuery(Sql);
            if (n == 0)
            {
                throw new DataException("Record doesn't exist OR LockVersion doesn't match!");
            }
            if(obj is DbObjectSmartUpdate)
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
                    object Key = dp.Dialect.ExecuteInsert(dp, sb, oi);
                    ObjectInfo.SetKey(obj, Key);
                    foreach(Type t2 in oi.CrossTables.Keys)
                    {
                        SetManyToManyRelation(oi, t2, Key, Scope<object>.Current);
                    }
                }, delegate(object o)
                {
                    SetBelongsToForeignKey(obj, o, oi.Handler.GetKeyValue(obj));
                    Save(o);
                });
            }
            else
            {
                SqlStatement Sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(Sql);
                ExecuteNonQuery(Sql);
            }
            if (DataSetting.CacheEnabled && oi.Cacheable && oi.HasOnePremarykey)
            {
                SetCachedObject(obj);
            }
        }

        private void SetManyToManyRelation(ObjectInfo oi, Type t, object Key1, object Key2)
        {
            if(oi.CrossTables.ContainsKey(t) && Key1 != null && Key2 != null)
            {
                CrossTable mt = oi.CrossTables[t];
                var sb = new InsertStatementBuilder(mt.Name);
                sb.Values.Add(new KeyValue(mt.ColumeName1, Key1));
                sb.Values.Add(new KeyValue(mt.ColumeName2, Key2));
                SqlStatement Sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(Sql);
                ExecuteNonQuery(Sql);
            }
        }

        private void RemoveManyToManyRelation(ObjectInfo oi, Type t, object Key1, object Key2)
        {
            if (oi.CrossTables.ContainsKey(t) && Key1 != null && Key2 != null)
            {
                CrossTable mt = oi.CrossTables[t];
                var sb = new DeleteStatementBuilder(mt.Name);
                WhereCondition c = CK.K[mt.ColumeName1] == Key1;
                c &= CK.K[mt.ColumeName2] == Key2;
                sb.Where.Conditions = c;
                SqlStatement Sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(Sql);
                ExecuteNonQuery(Sql);
            }
        }

        private static void SetBelongsToForeignKey(object obj, object subobj, object ForeignKey)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(subobj.GetType());
            MemberHandler mh = oi.GetBelongsTo(obj.GetType());
            if (mh != null)
            {
                var ho = mh.GetValue(subobj) as IBelongsTo;
                if (ho != null)
                {
                    ho.ForeignKey = ForeignKey;
                }
            }
        }

        public int Delete(object obj)
        {
            Type t = obj.GetType();
            TryCreateTable(t);
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            SqlStatement Sql = oi.Composer.GetDeleteStatement(Dialect, obj);
            oi.LogSql(Sql);
            int ret = 0;
            ProcessRelation(oi, false, obj, delegate(DataProvider dp)
            {
                ret += dp.ExecuteNonQuery(Sql);
                if(DataSetting.CacheEnabled && oi.Cacheable)
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
                SqlStatement Sql = sb.ToSqlStatement(Dialect);
                oi.LogSql(Sql);
                ret += ExecuteNonQuery(Sql);
            }
            return ret;
        }

        public int Delete<T>(WhereCondition iwc) where T : class, IDbObject
        {
            Type t = typeof(T);
            TryCreateTable(t);
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            SqlStatement Sql = oi.Composer.GetDeleteStatement(Dialect, iwc);
            oi.LogSql(Sql);
            return ExecuteNonQuery(Sql);
        }

        public void DropTable(Type DbObjectType)
        {
            DropTable(DbObjectType, true);
        }

        public void DropTable(Type DbObjectType, bool CatchException)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            string tn = oi.From.GetMainTableName();
            DropTable(tn, CatchException, oi);
            if (oi.HasSystemKey)
            {
                CommonHelper.IfCatchException(true, () => Dialect.ExecuteDropSequence(this, tn));
            }
            foreach (CrossTable mt in oi.CrossTables.Values)
            {
                DropTable(mt.Name, CatchException, oi);
            }
        }

        private void DropTable(string TableName, bool CatchException, ObjectInfo oi)
        {
            string s = "DROP TABLE " + Dialect.QuoteForTableName(TableName);
            var Sql = new SqlStatement(s);
            oi.LogSql(Sql);
            CommonHelper.IfCatchException(CatchException, () => ExecuteNonQuery(Sql));
            if (Driver.AutoCreateTable && TableNames != null)
            {
                TableNames.Remove(TableName.ToLower());
            }
        }

        public void DropAndCreate(Type DbObjectType)
        {
            DropTable(DbObjectType, true);
            Create(DbObjectType);
        }

        public void Create(Type DbObjectType)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            SqlStatement Sql = oi.Composer.GetCreateStatement(Dialect);
            oi.LogSql(Sql);
            ExecuteNonQuery(Sql);
            if (Driver.AutoCreateTable && TableNames != null)
            {
                TableNames.Add(oi.From.GetMainTableName().ToLower(), 1);
            }
        }

        public void CreateDeleteToTable(Type DbObjectType)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            CreateTableStatementBuilder sb = oi.Composer.GetCreateTableStatementBuilder();
            sb.TableName = oi.DeleteToTableName;
            sb.Columns.Add(new ColumnInfo("DeletedOn", typeof(DateTime), false, false, false, false, 0));
            SqlStatement Sql = sb.ToSqlStatement(Dialect);
            oi.LogSql(Sql);
            ExecuteNonQuery(Sql);
            if (Driver.AutoCreateTable && TableNames != null)
            {
                TableNames.Add(oi.DeleteToTableName.ToLower(), 1);
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
            if(oi1.KeyFields.Length <= 0 || oi2.KeyFields.Length <= 0)
            {
                throw new DataException("The relation table must have key column!");
            }
            CrossTable mt1 = oi1.CrossTables[t2];
            var cts = new CreateTableStatementBuilder(mt1.Name);
            var ls = new List<string> {mt1.ColumeName1, mt1.ColumeName2};
            ls.Sort();
            cts.Columns.Add(new ColumnInfo(ls[0], oi1.KeyFields[0].FieldType, false, false, false, false, 0));
            cts.Columns.Add(new ColumnInfo(ls[1], oi2.KeyFields[0].FieldType, false, false, false, false, 0));
            // add index
            cts.Indexes.Add(new DbIndex(null, false, (ASC)mt1.ColumeName1));
            cts.Indexes.Add(new DbIndex(null, false, (ASC)mt1.ColumeName2));
            // execute
            SqlStatement Sql = cts.ToSqlStatement(Dialect);
            oi1.LogSql(Sql);
            ExecuteNonQuery(Sql);
            if (Driver.AutoCreateTable && TableNames != null)
            {
                TableNames.Add(mt1.Name.ToLower(), 1);
            }
        }
    }
}
