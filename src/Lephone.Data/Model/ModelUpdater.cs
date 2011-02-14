using System;
using Lephone.Core;
using Lephone.Data.Builder;
using Lephone.Data.Definition;
using Lephone.Data.Model.Member;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model
{
    public abstract class ModelUpdater
    {
        protected DataProvider Provider;

        protected ModelUpdater(DataProvider provider)
        {
            this.Provider = provider;
        }

        private static void ClearUpdatedColumns(object obj)
        {
            if (obj is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)obj).m_InitUpdateColumns();
            }
        }

        public virtual int Delete(IDbObject obj)
        {
            var deleter = new ModelDeleter(obj);
            return deleter.Process();
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
                ctx.Provider.ExecuteNonQuery(sql);
            }
            ClearUpdatedColumns(obj);
        }

        private void InnerSave(object obj)
        {
            if (!Scope<SavedObjectList>.Current.Contains(obj))
            {
                ModelContext instance = ModelContext.GetInstance(obj.GetType());
                if (!instance.Info.HasOnePrimaryKey)
                {
                    throw new DataException("To call this function, the table must have one primary key.");
                }
                MemberHandler handler = instance.Info.KeyMembers[0];
                if (instance.Info.HasSystemKey)
                {
                    if (handler.UnsavedValue == null)
                    {
                        throw new DataException("To call this functionn, the UnsavedValue must be set.");
                    }
                    this.InnerSave(handler.UnsavedValue.Equals(handler.GetValue(obj)), obj);
                }
                else
                {
                    this.InnerSave(null == instance.Operator.GetObject(handler.GetValue(obj)), obj);
                }
            }
        }

        private void InnerSave(bool isInsert, object obj)
        {
            if (isInsert)
            {
                this.InnerInsert(obj);
            }
            else
            {
                this.InnerUpdate(obj);
            }
        }

        private void InnerUpdate(object obj)
        {
            Condition iwc = ModelContext.GetKeyWhereClause(obj);
            Type type = obj.GetType();
            ModelContext ctx = ModelContext.GetInstance(type);
            ctx.Operator.TryCreateTable();
            this.ProcessRelation(ctx, obj, delegate(DataProvider dp)
            {
                var update = obj as DbObjectSmartUpdate;
                if ((update != null) && (update.m_UpdateColumns != null))
                {
                    if (update.m_UpdateColumns.Count > 0)
                    {
                        this.InnerUpdate(obj, iwc, ctx, dp);
                    }
                }
                else
                {
                    this.InnerUpdate(obj, iwc, ctx, dp);
                }
            });
        }

        protected virtual void InnerUpdate(object obj, Condition iwc, ModelContext ctx, DataProvider dp)
        {
            SqlStatement updateStatement = ctx.Composer.GetUpdateStatement(obj, iwc);
            if (dp.ExecuteNonQuery(updateStatement) == 0)
            {
                throw new DataException("Record doesn't exist OR LockVersion doesn't match!");
            }
            ClearUpdatedColumns(obj);
            ctx.Composer.ProcessAfterSave(obj);
        }

        public void Insert(IDbObject obj)
        {
            UsingSavedObjectList(() => this.InnerInsert(obj));
        }

        private void ProcessChildren(ModelContext ctx, object obj, CallbackObjectHandler<object> processChild)
        {
            using (new Scope<object>(ctx.Handler.GetKeyValue(obj)))
            {
                foreach (MemberHandler handler in ctx.Info.RelationMembers)
                {
                    var ho = (ILazyLoading)handler.GetValue(obj);
                    if (ho.IsLoaded)
                    {
                        if (handler.Is.HasOne)
                        {
                            this.ProcessHasOne(ho, processChild);
                        }
                        else if (handler.Is.HasMany)
                        {
                            this.ProcessHasMany(ho, processChild);
                        }
                        else if (handler.Is.HasAndBelongsToMany)
                        {
                            ProcessHasAndBelongsToMany(ctx, obj, handler, ho, processChild);
                        }
                    }
                }
            }
        }

        private static void ProcessHasAndBelongsToMany(ModelContext ctx, object obj, MemberHandler f, ILazyLoading ho, CallbackObjectHandler<object> processChild)
        {
            object obj2 = ho.Read();
            if (obj2 != null)
            {
                CommonHelper.TryEnumerate(obj2, processChild);
            }
            var relations = (IHasAndBelongsToManyRelations)ho;
            foreach (object obj3 in relations.SavedNewRelations)
            {
                SetManyToManyRelation(ctx, f.FieldType.GetGenericArguments()[0], ctx.Handler.GetKeyValue(obj), obj3);
            }
            foreach (object obj4 in relations.RemovedRelations)
            {
                RemoveManyToManyRelation(ctx, f.FieldType.GetGenericArguments()[0], ctx.Handler.GetKeyValue(obj), obj4);
            }
        }

        private void ProcessHasMany(ILazyLoading ho, CallbackObjectHandler<object> processChild)
        {
            object obj2 = ho.Read();
            if (obj2 != null)
            {
                var many = (IHasMany)ho;
                foreach (object obj3 in many.RemovedValues)
                {
                    this.RelationSave(obj3);
                }
                CommonHelper.TryEnumerate(obj2, processChild);
            }
        }

        private void ProcessHasOne(ILazyLoading ho, CallbackObjectHandler<object> processChild)
        {
            object obj2 = ho.Read();
            if (obj2 == null)
            {
                var one = (IHasOne)ho;
                if (one.LastValue != null)
                {
                    this.RelationSave(one.LastValue);
                }
            }
            else
            {
                CommonHelper.TryEnumerate(obj2, processChild);
            }
        }

        private void ProcessRelation(ModelContext ctx, object obj, CallbackObjectHandler<DataProvider> processParent)
        {
            if (ctx.Info.HasAssociate)
            {
                DbEntry.UsingTransaction(delegate
                {
                    foreach (var f in ctx.Info.RelationMembers)
                    {
                        if (f.Is.BelongsTo)
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
                        processParent(ctx.Provider);
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
                processParent(ctx.Provider);
            }
        }

        private void RelationSave(object obj)
        {
            if (obj is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)obj).Save();
            }
            else
            {
                this.InnerSave(obj);
            }
        }

        private static void RemoveManyToManyRelation(ModelContext ctx, Type t, object key1, object key2)
        {
            if ((ctx.Info.CrossTables.ContainsKey(t) && (key1 != null)) && (key2 != null))
            {
                CrossTable table = ctx.Info.CrossTables[t];
                var builder = new DeleteStatementBuilder(table.Name);
                var condition = (Condition)(CK.K[table.ColumeName1] == key1);
                condition &= CK.K[table.ColumeName2] == key2;
                builder.Where.Conditions = condition;
                var sql = builder.ToSqlStatement(ctx);
                ctx.Provider.ExecuteNonQuery(sql);
            }
        }

        public void Save(IDbObject obj)
        {
            UsingSavedObjectList(() => this.InnerSave(obj));
        }

        private static void SetBelongsToForeignKey(object obj, object subobj, object foreignKey)
        {
            MemberHandler belongsTo = ModelContext.GetInstance(subobj.GetType()).Info.GetBelongsTo(obj.GetType());
            if (belongsTo != null)
            {
                var to = belongsTo.GetValue(subobj) as IBelongsTo;
                if (to != null)
                {
                    to.ForeignKey = foreignKey;
                }
            }
        }

        private static void SetManyToManyRelation(ModelContext ctx, Type t, object key1, object key2)
        {
            if ((ctx.Info.CrossTables.ContainsKey(t) && (key1 != null)) && (key2 != null))
            {
                CrossTable table = ctx.Info.CrossTables[t];
                var builder = new InsertStatementBuilder(table.Name);
                builder.Values.Add(new KeyValue(table.ColumeName1, key1));
                builder.Values.Add(new KeyValue(table.ColumeName2, key2));
                SqlStatement sql = builder.ToSqlStatement(ctx);
                ctx.Provider.ExecuteNonQuery(sql);
            }
        }

        public void Update(IDbObject obj)
        {
            UsingSavedObjectList(() => this.InnerUpdate(obj));
        }

        private static void UsingSavedObjectList(CallbackVoidHandler callback)
        {
            if (Scope<SavedObjectList>.Current == null)
            {
                using (new Scope<SavedObjectList>(new SavedObjectList()))
                {
                    callback();
                    return;
                }
            }
            callback();
        }
    }
}

