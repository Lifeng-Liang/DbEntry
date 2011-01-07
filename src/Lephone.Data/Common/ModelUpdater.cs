using System;
using Lephone.Data.Builder;
using Lephone.Data.Caching;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.Core;

namespace Lephone.Data.Common
{
    public class ModelUpdater
    {
        private static void UsingAvoidObjectList(CallbackVoidHandler callback)
        {
            if (Scope<AvoidObjectList>.Current == null)
            {
                using (new Scope<AvoidObjectList>(new AvoidObjectList()))
                {
                    callback();
                }
            }
            else
            {
                callback();
            }
        }

        #region Save

        public void Save(object obj)
        {
            UsingAvoidObjectList(() => InnerSave(obj));
        }

        private void RelationSave(object obj)
        {
            if(obj is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)obj).Save();
            }
            else
            {
                InnerSave(obj);
            }
        }

        private void InnerSave(object obj)
        {
            if (!Scope<AvoidObjectList>.Current.Contains(obj))
            {
                ObjectInfo oi = ObjectInfo.GetInstance(obj.GetType());
                if (!oi.HasOnePrimaryKey)
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
                    InnerSave(null == oi.Context.GetObject(obj.GetType(), k.GetValue(obj)), obj);
                }
            }
        }

        private void InnerSave(bool isInsert, object obj)
        {
            if (isInsert)
            {
                InnerInsert(obj);
            }
            else
            {
                InnerUpdate(obj);
            }
        }

        public void Insert(object obj)
        {
            UsingAvoidObjectList(() => InnerInsert(obj));
        }

        private void InnerInsert(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            oi.Context.TryCreateTable(t);
            InsertStatementBuilder sb = oi.Composer.GetInsertStatementBuilder(obj);
            if (oi.HasSystemKey)
            {
                ProcessRelation(oi, obj, delegate(DataProvider dp)
                {
                    object key = dp.Dialect.ExecuteInsert(dp, sb, oi);
                    ObjectInfo.SetKey(obj, key);
                    foreach (Type t2 in oi.CrossTables.Keys)
                    {
                        SetManyToManyRelation(oi, t2, key, Scope<object>.Current);
                    }
                });
            }
            else
            {
                SqlStatement sql = sb.ToSqlStatement(oi);
                oi.Context.ExecuteNonQuery(sql);
            }
            ClearUpdatedColumns(obj);
            if (DataSettings.CacheEnabled && oi.Cacheable && oi.HasOnePrimaryKey)
            {
                oi.Context.SetCachedObject(obj);
            }
        }

        public void Update(object obj)
        {
            UsingAvoidObjectList(() => InnerUpdate(obj));
        }

        private void InnerUpdate(object obj)
        {
            var iwc = ObjectInfo.GetKeyWhereClause(obj);
            Type t = obj.GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            oi.Context.TryCreateTable(t);
            ProcessRelation(oi, obj, delegate(DataProvider dp)
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
            });
        }

        private static void InnerUpdate(object obj, Condition iwc, ObjectInfo oi, DataProvider dp)
        {
            SqlStatement sql = oi.Composer.GetUpdateStatement(obj, iwc);
            int n = dp.ExecuteNonQuery(sql);
            if (n == 0)
            {
                throw new DataException("Record doesn't exist OR LockVersion doesn't match!");
            }
            ClearUpdatedColumns(obj);
            oi.Composer.ProcessAfterSave(obj);

            if (DataSettings.CacheEnabled && oi.Cacheable && oi.HasOnePrimaryKey)
            {
                oi.Context.SetCachedObject(obj);
            }
        }

        private static void ClearUpdatedColumns(object obj)
        {
            if(obj is DbObjectSmartUpdate)
            {
                var o = (DbObjectSmartUpdate)obj;
                o.m_InitUpdateColumns();
            }
        }

        private void ProcessRelation(ObjectInfo oi, object obj, CallbackObjectHandler<DataProvider> processParent)
        {
            if (oi.HasAssociate)
            {
                oi.Context.UsingTransaction(delegate
                {
                    foreach (var f in oi.RelationFields)
                    {
                        if (f.IsBelongsTo)
                        {
                            var ho = (ILazyLoading)f.GetValue(obj);
                            if (ho.IsLoaded)
                            {
                                object llo = ho.Read();
                                if (llo != null)
                                {
                                    RelationSave(llo);
                                }
                            }
                        }
                    }
                    if (!Scope<AvoidObjectList>.Current.Contains(obj))
                    {
                        Scope<AvoidObjectList>.Current.Add(obj);
                        processParent(oi.Context);
                        ProcessChildren(oi, obj, o =>
                        {
                            SetBelongsToForeignKey(obj, o, oi.Handler.GetKeyValue(obj));
                            RelationSave(o);
                        });
                    }
                });
            }
            else
            {
                processParent(oi.Context);
            }
        }

        private void ProcessChildren(ObjectInfo oi, object obj, CallbackObjectHandler<object> processChild)
        {
            object mkey = oi.Handler.GetKeyValue(obj);
            using (new Scope<object>(mkey))
            {
                foreach (MemberHandler f in oi.RelationFields)
                {
                    var ho = (ILazyLoading)f.GetValue(obj);
                    if (ho.IsLoaded)
                    {
                        if (f.IsHasOne)
                        {
                            ProcessHasOne(ho, processChild);
                        }
                        else if (f.IsHasMany)
                        {
                            ProcessHasMany(ho, processChild);
                        }
                        else if (f.IsHasAndBelongsToMany)
                        {
                            ProcessHasAndBelongsToMany(oi, obj, f, ho, processChild);
                        }
                    }
                }
            }
        }

        private static void ProcessHasAndBelongsToMany(ObjectInfo oi, object obj, MemberHandler f, ILazyLoading ho, CallbackObjectHandler<object> processChild)
        {
            object llo = ho.Read();
            if (llo != null)
            {
                CommonHelper.TryEnumerate(llo, processChild);
            }
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

        private void ProcessHasMany(ILazyLoading ho, CallbackObjectHandler<object> processChild)
        {
            object llo = ho.Read();
            if (llo != null)
            {
                var ho1 = (IHasMany)ho;
                foreach (object item in ho1.RemovedValues)
                {
                    RelationSave(item);
                }
                CommonHelper.TryEnumerate(llo, processChild);
            }
        }

        private void ProcessHasOne(ILazyLoading ho, CallbackObjectHandler<object> processChild)
        {
            object llo = ho.Read();
            if (llo == null)
            {
                var ho1 = (IHasOne)ho;
                if (ho1.LastValue != null)
                {
                    RelationSave(ho1.LastValue);
                }
            }
            else
            {
                CommonHelper.TryEnumerate(llo, processChild);
            }
        }

        #endregion

        #region Delete

        public int Delete(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            oi.Context.TryCreateTable(t);
            SqlStatement sql = oi.Composer.GetDeleteStatement(obj);
            int ret = 0;
            ProcessRelation2(oi, obj, delegate(DataProvider dp)
            {
                ret += dp.ExecuteNonQuery(sql);
                if (DataSettings.CacheEnabled && oi.Cacheable)
                {
                    CacheProvider.Instance.Remove(KeyGenerator.Instance[obj]);
                }
                ret += DeleteRelation(oi, obj);
            });
            if (oi.KeyFields[0].UnsavedValue != null)
            {
                oi.KeyFields[0].SetValue(obj, oi.KeyFields[0].UnsavedValue);
            }
            return ret;
        }

        private static int DeleteRelation(ObjectInfo oi, object obj)
        {
            int ret = 0;
            foreach (CrossTable mt in oi.CrossTables.Values)
            {
                var sb = new DeleteStatementBuilder(mt.Name);
                sb.Where.Conditions = CK.K[mt.ColumeName1] == oi.Handler.GetKeyValue(obj);
                SqlStatement sql = sb.ToSqlStatement(oi);
                ret += oi.Context.ExecuteNonQuery(sql);
            }
            return ret;
        }

        private void ProcessRelation2(ObjectInfo oi, object obj, CallbackObjectHandler<DataProvider> processParent)
        {
            if (oi.HasAssociate)
            {
                oi.Context.UsingTransaction(delegate
                {
                    ProcessChildren2(oi, obj, o => Delete(o));
                    processParent(oi.Context);
                });
            }
            else
            {
                processParent(oi.Context);
            }
        }

        private void ProcessChildren2(ObjectInfo oi, object obj, CallbackObjectHandler<object> processChild)
        {
            object mkey = oi.Handler.GetKeyValue(obj);
            using (new Scope<object>(mkey))
            {
                foreach (MemberHandler f in oi.RelationFields)
                {
                    var ho = (ILazyLoading)f.GetValue(obj);
                    if (ho.IsLoaded)
                    {
                        if (f.IsHasOne)
                        {
                            object llo = ho.Read();
                            if (llo == null)
                            {
                                var ho1 = (IHasOne)ho;
                                if (ho1.LastValue != null)
                                {
                                    RelationSave(ho1.LastValue);
                                }
                            }
                            else
                            {
                                CommonHelper.TryEnumerate(llo, processChild);
                            }
                        }
                        else if (f.IsHasMany)
                        {
                            object llo = ho.Read();
                            if (llo != null)
                            {
                                var ho1 = (IHasMany)ho;
                                foreach (object item in ho1.RemovedValues)
                                {
                                    RelationSave(item);
                                }
                                CommonHelper.TryEnumerate(llo, processChild);
                            }
                        }
                        else if (f.IsHasAndBelongsToMany)
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
                }
            }
        }

        #endregion

        #region Help functions

        private static void SetManyToManyRelation(ObjectInfo oi, Type t, object key1, object key2)
        {
            if (oi.CrossTables.ContainsKey(t) && key1 != null && key2 != null)
            {
                CrossTable mt = oi.CrossTables[t];
                var sb = new InsertStatementBuilder(mt.Name);
                sb.Values.Add(new KeyValue(mt.ColumeName1, key1));
                sb.Values.Add(new KeyValue(mt.ColumeName2, key2));
                SqlStatement sql = sb.ToSqlStatement(oi);
                oi.Context.ExecuteNonQuery(sql);
            }
        }

        private static void RemoveManyToManyRelation(ObjectInfo oi, Type t, object key1, object key2)
        {
            if (oi.CrossTables.ContainsKey(t) && key1 != null && key2 != null)
            {
                CrossTable mt = oi.CrossTables[t];
                var sb = new DeleteStatementBuilder(mt.Name);
                Condition c = CK.K[mt.ColumeName1] == key1;
                c &= CK.K[mt.ColumeName2] == key2;
                sb.Where.Conditions = c;
                SqlStatement sql = sb.ToSqlStatement(oi);
                oi.Context.ExecuteNonQuery(sql);
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

        #endregion
    }
}
