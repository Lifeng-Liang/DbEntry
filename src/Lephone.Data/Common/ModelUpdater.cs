using System;
using Lephone.Core;
using Lephone.Data.Builder;
using Lephone.Data.Caching;
using Lephone.Data.Definition;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Common
{
    public abstract class ModelUpdater : DataProvider
    {
        protected ModelUpdater(DbDriver driver) : base(driver)
        {
        }

        private static void UsingSavedObjectList(CallbackVoidHandler callback)
        {
            if (Scope<SavedObjectList>.Current == null)
            {
                using (new Scope<SavedObjectList>(new SavedObjectList()))
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
            UsingSavedObjectList(() => InnerSave(obj));
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
            if (!Scope<SavedObjectList>.Current.Contains(obj))
            {
                var ctx = ModelContext.GetInstance(obj.GetType());
                if (!ctx.Info.HasOnePrimaryKey)
                {
                    throw new DataException("To call this function, the table must have one primary key.");
                }
                MemberHandler k = ctx.Info.KeyFields[0];
                if (ctx.Info.HasSystemKey)
                {
                    if (k.UnsavedValue == null)
                    {
                        throw new DataException("To call this functionn, the UnsavedValue must be set.");
                    }
                    InnerSave(k.UnsavedValue.Equals(k.GetValue(obj)), obj);
                }
                else
                {
                    InnerSave(null == ctx.Operator.GetObject(k.GetValue(obj)), obj);
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
            UsingSavedObjectList(() => InnerInsert(obj));
        }

        protected virtual void InnerInsert(object obj)
        {
            Type t = obj.GetType();
            var ctx = ModelContext.GetInstance(t);
            ctx.Operator.TryCreateTable();
            InsertStatementBuilder sb = ctx.Composer.GetInsertStatementBuilder(obj);
            if (ctx.Info.HasSystemKey)
            {
                ProcessRelation(ctx, obj, delegate(DataProvider dp)
                {
                    object key = dp.Dialect.ExecuteInsert(dp, sb, ctx);
                    ModelContext.SetKey(obj, key);
                    foreach (Type t2 in ctx.Info.CrossTables.Keys)
                    {
                        SetManyToManyRelation(ctx, t2, key, Scope<object>.Current);
                    }
                });
            }
            else
            {
                SqlStatement sql = sb.ToSqlStatement(ctx);
                ctx.Operator.ExecuteNonQuery(sql);
            }
            ClearUpdatedColumns(obj);
        }

        public void Update(object obj)
        {
            UsingSavedObjectList(() => InnerUpdate(obj));
        }

        private void InnerUpdate(object obj)
        {
            var iwc = ModelContext.GetKeyWhereClause(obj);
            Type t = obj.GetType();
            var ctx = ModelContext.GetInstance(t);
            ctx.Operator.TryCreateTable();
            ProcessRelation(ctx, obj, delegate(DataProvider dp)
            {
                var to = obj as DbObjectSmartUpdate;
                if (to != null && to.m_UpdateColumns != null)
                {
                    if (to.m_UpdateColumns.Count > 0)
                    {
                        InnerUpdate(obj, iwc, ctx, dp);
                    }
                }
                else
                {
                    InnerUpdate(obj, iwc, ctx, dp);
                }
            });
        }

        protected virtual void InnerUpdate(object obj, Condition iwc, ModelContext ctx, DataProvider dp)
        {
            SqlStatement sql = ctx.Composer.GetUpdateStatement(obj, iwc);
            int n = dp.ExecuteNonQuery(sql);
            if (n == 0)
            {
                throw new DataException("Record doesn't exist OR LockVersion doesn't match!");
            }
            ClearUpdatedColumns(obj);
            ctx.Composer.ProcessAfterSave(obj);
        }

        private static void ClearUpdatedColumns(object obj)
        {
            if(obj is DbObjectSmartUpdate)
            {
                var o = (DbObjectSmartUpdate)obj;
                o.m_InitUpdateColumns();
            }
        }

        private void ProcessRelation(ModelContext ctx, object obj, CallbackObjectHandler<DataProvider> processParent)
        {
            if (ctx.Info.HasAssociate)
            {
                DbEntry.UsingTransaction(delegate
                {
                    foreach (var f in ctx.Info.RelationFields)
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
                    if (!Scope<SavedObjectList>.Current.Contains(obj))
                    {
                        Scope<SavedObjectList>.Current.Add(obj);
                        processParent(ctx.Operator);
                        ProcessChildren(ctx, obj, o =>
                        {
                            SetBelongsToForeignKey(obj, o, ctx.Handler.GetKeyValue(obj));
                            RelationSave(o);
                        });
                    }
                });
            }
            else
            {
                processParent(ctx.Operator);
            }
        }

        private void ProcessChildren(ModelContext ctx, object obj, CallbackObjectHandler<object> processChild)
        {
            object mkey = ctx.Handler.GetKeyValue(obj);
            using (new Scope<object>(mkey))
            {
                foreach (MemberHandler f in ctx.Info.RelationFields)
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
                            ProcessHasAndBelongsToMany(ctx, obj, f, ho, processChild);
                        }
                    }
                }
            }
        }

        private static void ProcessHasAndBelongsToMany(ModelContext ctx, object obj, MemberHandler f, ILazyLoading ho, CallbackObjectHandler<object> processChild)
        {
            object llo = ho.Read();
            if (llo != null)
            {
                CommonHelper.TryEnumerate(llo, processChild);
            }
            var so = (IHasAndBelongsToManyRelations)ho;
            foreach (object n in so.SavedNewRelations)
            {
                SetManyToManyRelation(ctx, f.FieldType.GetGenericArguments()[0], ctx.Handler.GetKeyValue(obj), n);
            }
            foreach (object n in so.RemovedRelations)
            {
                RemoveManyToManyRelation(ctx, f.FieldType.GetGenericArguments()[0], ctx.Handler.GetKeyValue(obj), n);
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
            var ctx = ModelContext.GetInstance(t);
            ctx.Operator.TryCreateTable();
            SqlStatement sql = ctx.Composer.GetDeleteStatement(obj);
            int ret = 0;
            ProcessRelation2(ctx, obj, delegate(DataProvider dp)
            {
                ret += dp.ExecuteNonQuery(sql);
                if (DataSettings.CacheEnabled && ctx.Info.Cacheable)
                {
                    CacheProvider.Instance.Remove(KeyGenerator.Instance[obj]);
                }
                ret += DeleteRelation(ctx, obj);
            });
            if (ctx.Info.KeyFields[0].UnsavedValue != null)
            {
                ctx.Info.KeyFields[0].SetValue(obj, ctx.Info.KeyFields[0].UnsavedValue);
            }
            return ret;
        }

        private static int DeleteRelation(ModelContext ctx, object obj)
        {
            int ret = 0;
            foreach (CrossTable mt in ctx.Info.CrossTables.Values)
            {
                var sb = new DeleteStatementBuilder(mt.Name);
                sb.Where.Conditions = CK.K[mt.ColumeName1] == ctx.Handler.GetKeyValue(obj);
                SqlStatement sql = sb.ToSqlStatement(ctx);
                ret += ctx.Operator.ExecuteNonQuery(sql);
            }
            return ret;
        }

        private void ProcessRelation2(ModelContext ctx, object obj, CallbackObjectHandler<DataProvider> processParent)
        {
            if (ctx.Info.HasAssociate)
            {
                DbEntry.UsingTransaction(delegate
                {
                    ProcessChildren2(ctx, obj, o => Delete(o));
                    processParent(ctx.Operator);
                });
            }
            else
            {
                processParent(ctx.Operator);
            }
        }

        private void ProcessChildren2(ModelContext ctx, object obj, CallbackObjectHandler<object> processChild)
        {
            object mkey = ctx.Handler.GetKeyValue(obj);
            using (new Scope<object>(mkey))
            {
                foreach (MemberHandler f in ctx.Info.RelationFields)
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
                                SetManyToManyRelation(ctx, f.FieldType.GetGenericArguments()[0], ctx.Handler.GetKeyValue(obj), n);
                            }
                            foreach (object n in so.RemovedRelations)
                            {
                                RemoveManyToManyRelation(ctx, f.FieldType.GetGenericArguments()[0], ctx.Handler.GetKeyValue(obj), n);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Help functions

        private static void SetManyToManyRelation(ModelContext ctx, Type t, object key1, object key2)
        {
            if (ctx.Info.CrossTables.ContainsKey(t) && key1 != null && key2 != null)
            {
                CrossTable mt = ctx.Info.CrossTables[t];
                var sb = new InsertStatementBuilder(mt.Name);
                sb.Values.Add(new KeyValue(mt.ColumeName1, key1));
                sb.Values.Add(new KeyValue(mt.ColumeName2, key2));
                SqlStatement sql = sb.ToSqlStatement(ctx);
                ctx.Operator.ExecuteNonQuery(sql);
            }
        }

        private static void RemoveManyToManyRelation(ModelContext ctx, Type t, object key1, object key2)
        {
            if (ctx.Info.CrossTables.ContainsKey(t) && key1 != null && key2 != null)
            {
                CrossTable mt = ctx.Info.CrossTables[t];
                var sb = new DeleteStatementBuilder(mt.Name);
                Condition c = CK.K[mt.ColumeName1] == key1;
                c &= CK.K[mt.ColumeName2] == key2;
                sb.Where.Conditions = c;
                SqlStatement sql = sb.ToSqlStatement(ctx);
                ctx.Operator.ExecuteNonQuery(sql);
            }
        }

        private static void SetBelongsToForeignKey(object obj, object subobj, object foreignKey)
        {
            var ctx = ModelContext.GetInstance(subobj.GetType());
            var mh = ctx.Info.GetBelongsTo(obj.GetType());
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
