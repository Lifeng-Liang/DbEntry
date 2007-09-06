
#region usings

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using org.hanzify.llf.util;
using org.hanzify.llf.util.Logging;
using org.hanzify.llf.Data.Definition;
using org.hanzify.llf.Data.QuerySyntax;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.Builder.Clause;

#endregion

namespace org.hanzify.llf.Data
{
    public class DbContext : DataProvider
    {
        private Dictionary<string, int> TableNames = null;
        // for remoting only
        public DbContext() : this(EntryConfig.Default) { }

        public DbContext(string Prefix) : this(EntryConfig.GetDriver(Prefix)) { }

        public DbContext(DbDriver driver) : base(driver) { }

        private void TryCreateTable(Type DbObjectType)
        {
            if (DataSetting.AutoCreateTable)
            {
                if (TableNames == null)
                {
                    InitTableNames();
                }
                ObjectInfo ii = DbObjectHelper.GetObjectInfo(DbObjectType);
                string Name = ii.From.GetMainTableName();
                if (!TableNames.ContainsKey(Name.ToLower()))
                {
                    Create(DbObjectType);
                    if (ii.ManyToManyMediTableName != null && !TableNames.ContainsKey(ii.ManyToManyMediTableName.ToLower()))
                    {
                        CreateManyToManyMediTable(DbObjectType, GetManyToManyRelationType(ii));
                    }
                }
            }
        }

        private Type GetManyToManyRelationType(ObjectInfo ii)
        {
            foreach (MemberHandler m in ii.Fields)
            {
                if (m.IsHasAndBelongsToMany)
                {
                    return m.FieldType.GetGenericArguments()[0];
                }
            }
            throw new DbEntryException("impossible");
        }

        private void InitTableNames()
        {
            TableNames = new Dictionary<string, int>();
            foreach (string s in GetTableNames())
            {
                TableNames.Add(s.ToLower(), 1);
            }
        }

        public IWhere<T> From<T>()
        {
            return new QueryContent<T>(this);
        }

        public long GetResultCount(Type DbObjectType, WhereCondition iwc)
        {
            TryCreateTable(DbObjectType);
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(DbObjectType);
            SqlStatement Sql = oi.Composer.GetResultCountStatement(this.Dialect, iwc);
            Logger.SQL.Trace(Sql);
            object ro = this.ExecuteScalar(Sql);
            return Convert.ToInt64(ro);
        }

        public DbObjectList<GroupByObject<T1>> GetGroupBy<T1>(Type DbObjectType, WhereCondition iwc, OrderBy order, string ColumnName)
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(DbObjectType);
            SqlStatement Sql = oi.Composer.GetGroupByStatement(this.Dialect, iwc, order, ColumnName);
            Logger.SQL.Trace(Sql);
            DbObjectList<GroupByObject<T1>> list = new DbObjectList<GroupByObject<T1>>();
            IProcessor ip = GetListProcessor(list);
            DataLoadDirect(ip, typeof(GroupByObject<T1>), DbObjectType, Sql, true);
            return list;
        }

        public DbObjectList<T> ExecuteList<T>(string SqlStr)
        {
            return ExecuteList<T>(new SqlStatement(SqlStr));
        }

        public DbObjectList<T> ExecuteList<T>(string SqlStr, params object[] os)
        {
            return ExecuteList<T>(GetSqlStatement(SqlStr, os));
        }

        public DbObjectList<T> ExecuteList<T>(SqlStatement Sql)
        {
            DbObjectList<T> ret = new DbObjectList<T>();
            FillCollection(ret, typeof(T), Sql);
            return ret;
        }

        protected IProcessor GetListProcessor(IList il)
        {
            if (DataSetting.MaxRecords == 0)
            {
                return new ListInserter(il);
            }
            else
            {
                return new LimitedListInserter(il);
            }
        }

        public void FillCollection(IList list, Type DbObjectType, SqlStatement Sql)
        {
            IProcessor ip = GetListProcessor(list);
            DataLoad(ip, DbObjectType, Sql);
        }

        public void FillCollection(IList list, Type DbObjectType, WhereCondition iwc, OrderBy oc, Range lc)
        {
            IProcessor ip = GetListProcessor(list);
            DataLoad(ip, DbObjectType, null, iwc, oc, lc);
        }

        public void FillCollection(IList list, Type DbObjectType, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            IProcessor ip = GetListProcessor(list);
            DataLoad(ip, DbObjectType, from, iwc, oc, lc);
        }

        public void DataLoad(IProcessor ip, Type DbObjectType, SqlStatement Sql)
        {
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(DbObjectType);
            if (ii.AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
            DataLoadDirect(ip, DbObjectType, Sql, false);
        }

        public void DataLoad(IProcessor ip, Type DbObjectType, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(DbObjectType);
            SqlStatement Sql = ii.Composer.GetSelectStatement(this.Dialect, from, iwc, oc, lc);
            if (ii.AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
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
            if (this.Dialect.SupportsRangeStartIndex && EndIndex > 0)
            {
                EndIndex = EndIndex - StartIndex + 1;
                StartIndex = 1;
            }
            ExecuteDataReader(Sql, delegate(IDataReader dr)
            {
                int Count = 0;
                while (dr.Read())
                {
                    Count++;
                    if (Count >= StartIndex)
                    {
                        object di = DbObjectHelper.CreateObject(this, ReturnType, dr, UseIndex);
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

        public T GetObject<T>(object key)
        {
            return (T)GetObject(typeof(T), key);
        }

        public T GetObject<T>(WhereCondition c)
        {
            return (T)GetObject(typeof(T), c, null, null);
        }

        public T GetObject<T>(WhereCondition c, OrderBy ob)
        {
            return (T)GetObject(typeof(T), c, ob);
        }

        public object GetObject(Type t, object key)
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(t);
            if (oi.KeyFields != null && oi.KeyFields.Length == 1)
            {
                string keyname = oi.KeyFields[0].Name;
                return GetObject(t, CK.K[keyname] == key, null, null);
            }
            throw new DbEntryException("To call this function, the table must have one premary key.");
        }

        public object GetObject(Type t, WhereCondition c, OrderBy ob)
        {
            return GetObject(t, c, ob, new Range(1, 1));
        }

        private object GetObject(Type t, WhereCondition c, OrderBy ob, Range r)
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
            CommonHelper.TryEnumerate(obj, delegate(object o)
            {
                InnerSave(o);
            });
        }

        private void InnerSave(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(t);
            if (!ii.HasOnePremarykey)
            {
                throw new DbEntryException("To call this function, the table must have one primary key.");
            }
            MemberHandler k = ii.KeyFields[0];
            if (k.IsDbGenerate)
            {
                if (k.UnsavedValue == null)
                {
                    throw new DbEntryException("To call this functionn, the UnsavedValue must be set.");
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

        private void ProcessAssociate(ObjectInfo ii, bool ParentFirst, object obj,
            CallbackObjectHandler<DataProvider> e1, CallbackObjectHandler<object> e2)
        {
            if (ii.HasAssociate)
            {
                UsingExistedTransaction(delegate()
                {
                    if (ParentFirst) { e1(this); }
                    object mkey = ii.KeyFields[0].GetValue(obj);
                    using (new Scope<object>(mkey))
                    {
                        foreach (MemberHandler f in ii.RelationFields)
                        {
                            ILazyLoading ho = (ILazyLoading)f.GetValue(obj);
                            ho.IsLoaded = true;
                            if (f.IsHasOne || f.IsHasMany || (f.IsHasAndBelongsToMany && ParentFirst))
                            {
                                object llo = ho.Read();
                                CommonHelper.TryEnumerate(llo, e2);
                            }
                            if (f.IsHasAndBelongsToMany)
                            {
                                ISavedNewRelations so = ho as ISavedNewRelations;
                                foreach (long n in so.SavedNewRelations)
                                {
                                    SetManyToManyAssociate(ii, (obj as DbObject).Id, n);
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
            Update(obj, DbObjectHelper.GetKeyWhereClause(obj));
        }

        private void Update(object obj, WhereCondition iwc)
        {
            Type t = obj.GetType();
            TryCreateTable(t);
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(t);
            ProcessAssociate(ii, true, obj, delegate(DataProvider dp)
            {
                DbObjectSmartUpdate to = obj as DbObjectSmartUpdate;
                if (to != null && to.m_UpdateColumns != null)
                {
                    if (to.m_UpdateColumns.Count > 0)
                    {
                        InnerUpdate(obj, iwc, ii, dp);
                    }
                }
                else
                {
                    InnerUpdate(obj, iwc, ii, dp);
                }
            }, delegate(object o)
            {
                SetBelongsToForeignKey(obj, o, ii.KeyFields[0].GetValue(obj));
                Save(o);
            });
        }

        private void InnerUpdate(object obj, WhereCondition iwc, ObjectInfo ii, DataProvider dp)
        {
            SqlStatement Sql = ii.Composer.GetUpdateStatement(this.Dialect, obj, iwc);
            if (ii.AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
            dp.ExecuteNonQuery(Sql);
        }

        public void Insert(object obj)
        {
            Type t = obj.GetType();
            TryCreateTable(t);
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(t);
            InsertStatementBuilder sb = ii.Composer.GetInsertStatementBuilder(obj);
            if (ii.HasSystemKey)
            {
                ProcessAssociate(ii, true, obj, delegate(DataProvider dp)
                {
                    object Key = dp.Dialect.ExecuteInsert(dp, sb, ii);
                    DbObjectHelper.SetKey(obj, Key);
                    SetManyToManyAssociate(ii, Key, Scope<object>.Current);
                }, delegate(object o)
                {
                    SetBelongsToForeignKey(obj, o, ii.Handler.GetKeyValue(obj));
                    Save(o);
                });
            }
            else
            {
                SqlStatement Sql = sb.ToSqlStatement(this.Dialect);
                if (ii.AllowSqlLog)
                {
                    Logger.SQL.Trace(Sql);
                }
                this.ExecuteNonQuery(Sql);
            }
        }

        private void SetManyToManyAssociate(ObjectInfo ii, object Key1, object Key2)
        {
            if (ii.ManyToManyMediTableName != null && Key1 != null && Key2 != null)
            {
                InsertStatementBuilder sb = new InsertStatementBuilder(ii.ManyToManyMediTableName);
                sb.Values.Add(new KeyValue(ii.ManyToManyMediColumeName1, Key1));
                sb.Values.Add(new KeyValue(ii.ManyToManyMediColumeName2, Key2));
                SqlStatement Sql = sb.ToSqlStatement(this.Dialect);
                if (ii.AllowSqlLog)
                {
                    Logger.SQL.Trace(Sql);
                }
                ExecuteNonQuery(Sql);
            }
        }

        private void SetBelongsToForeignKey(object obj, object subobj, object ForeignKey)
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(subobj.GetType());
            MemberHandler mh = oi.GetBelongsTo(obj.GetType());
            if (mh != null)
            {
                Definition.IBelongsTo ho = mh.GetValue(subobj) as Definition.IBelongsTo;
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
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(t);
            SqlStatement Sql = ii.Composer.GetDeleteStatement(this.Dialect, obj);
            if (ii.AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
            int ret = 0;
            ProcessAssociate(ii, false, obj, delegate(DataProvider dp)
            {
                ret += dp.ExecuteNonQuery(Sql);
                ret += DeleteAssociate(ii, obj);
            }, delegate(object o)
            {
                Delete(o);
            });
            if (ii.KeyFields[0].UnsavedValue != null)
            {
                ii.KeyFields[0].SetValue(obj, ii.KeyFields[0].UnsavedValue);
            }
            return ret;
        }

        private int DeleteAssociate(ObjectInfo ii, object obj)
        {
            if (ii.ManyToManyMediTableName != null)
            {
                long Id = ((DbObject)obj).Id;
                DeleteStatementBuilder sb = new DeleteStatementBuilder(ii.ManyToManyMediTableName);
                sb.Where.Conditions = CK.K[ii.ManyToManyMediColumeName1] == Id;
                SqlStatement Sql = sb.ToSqlStatement(this.Dialect);
                if (ii.AllowSqlLog)
                {
                    Logger.SQL.Trace(Sql);
                }
                return ExecuteNonQuery(Sql);
            }
            return 0;
        }

        public int Delete<T>(WhereCondition iwc)
        {
            Type t = typeof(T);
            TryCreateTable(t);
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(t);
            SqlStatement Sql = ii.Composer.GetDeleteStatement(this.Dialect, iwc);
            if (ii.AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
            return this.ExecuteNonQuery(Sql);
        }

        public void DropTable(Type DbObjectType)
        {
            DropTable(DbObjectType, true);
        }

        public void DropTable(Type DbObjectType, bool CatchException)
        {
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(DbObjectType);
            string tn = ii.From.GetMainTableName();
            DropTable(tn, CatchException, ii.AllowSqlLog);
            CommonHelper.IfCatchException(true, delegate()
            {
                Dialect.ExecuteDropSequence(this, tn);
            });
            if (ii.ManyToManyMediTableName != null)
            {
                DropTable(ii.ManyToManyMediTableName, CatchException, ii.AllowSqlLog);
            }
        }

        private void DropTable(string TableName, bool CatchException, bool AllowSqlLog)
        {
            string s = "Drop Table " + this.Dialect.QuoteForTableName(TableName);
            SqlStatement Sql = new SqlStatement(s);
            if (AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
            CommonHelper.IfCatchException(CatchException, delegate()
            {
                this.ExecuteNonQuery(Sql);
            });
            if (DataSetting.AutoCreateTable && TableNames != null)
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
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(DbObjectType);
            SqlStatement Sql = ii.Composer.GetCreateStatement(this.Dialect);
            if (ii.AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
            this.ExecuteNonQuery(Sql);
            if (DataSetting.AutoCreateTable && TableNames != null)
            {
                TableNames.Add(ii.From.GetMainTableName().ToLower(), 1);
            }
        }

        public void CreateManyToManyMediTable(Type t1, Type t2)
        {
            ObjectInfo oi1 = DbObjectHelper.GetObjectInfo(t1);
            ObjectInfo oi2 = DbObjectHelper.GetObjectInfo(t2);
            if (oi1.ManyToManyMediTableName == null || oi2.ManyToManyMediTableName == null || oi1.ManyToManyMediTableName != oi2.ManyToManyMediTableName)
            {
                throw new DbEntryException("They are not many to many relation ship classes!");
            }
            CreateTableStatementBuilder cts = new CreateTableStatementBuilder(oi1.ManyToManyMediTableName);
            List<string> ls = new List<string>();
            ls.Add(oi1.ManyToManyMediColumeName1);
            ls.Add(oi1.ManyToManyMediColumeName2);
            ls.Sort();
            cts.Columns.Add(new ColumnInfo(ls[0], typeof(long), false, false, false, false, 0));
            cts.Columns.Add(new ColumnInfo(ls[1], typeof(long), false, false, false, false, 0));
            // add index
            cts.Indexes.Add(new DbIndex(null, false, (ASC)oi1.ManyToManyMediColumeName1));
            cts.Indexes.Add(new DbIndex(null, false, (ASC)oi1.ManyToManyMediColumeName2));
            // execute
            SqlStatement Sql = cts.ToSqlStatement(this.Dialect);
            if (oi1.AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
            this.ExecuteNonQuery(Sql);
            if (DataSetting.AutoCreateTable && TableNames != null)
            {
                TableNames.Add(oi1.ManyToManyMediTableName.ToLower(), 1);
            }
        }
    }
}
