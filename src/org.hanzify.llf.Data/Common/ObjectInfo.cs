
#region usings

using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;
using Lephone.Data.Builder.Clause;
using Lephone.Util.Logging;
using Lephone.Util;

#endregion

namespace Lephone.Data.Common
{
	public class ObjectInfo : FlyweightBase<Type, ObjectInfo>
	{
        #region GetInstance

        public new static ObjectInfo GetInstance(Type DbObjectType)
        {
            Type t = (DbObjectType.IsAbstract) ? DynamicObject.GetImplType(DbObjectType) : DbObjectType;
            ObjectInfo oi = FlyweightBase<Type, ObjectInfo>.GetInstance(t);
            if (oi.BaseType == null)
            {
                oi.BaseType = DbObjectType;
            }
            return oi;
        }

        internal static ObjectInfo GetSimpleInstance(Type DbObjectType)
        {
            Type t = (DbObjectType.IsAbstract) ? DynamicObject.GetImplType(DbObjectType) : DbObjectType;
            if (dic.ContainsKey(t))
            {
                return dic[t];
            }
            else
            {
                ObjectInfo oi = new ObjectInfo();
                oi.InitBySimpleMode(t);
                return oi;
            }
        }

        protected override void Init(Type t)
        {
            InitBySimpleMode(t);
            // binding QueryComposer
            if (!string.IsNullOrEmpty(SoftDeleteColumnName))
            {
                Composer = new SoftDeleteQueryComposer(this, SoftDeleteColumnName);
            }
            else if (!string.IsNullOrEmpty(DeleteToTableName))
            {
                Composer = new DeleteToQueryComposer(this);
            }
            else if (LockVersion != null)
            {
                Composer = new OptimisticLockingQueryComposer(this);
            }
            else
            {
                Composer = new QueryComposer(this);
            }
            // binding DbObjectHandler
            if (DataSetting.ObjectHandlerType == HandlerType.Emit
                || (DataSetting.ObjectHandlerType == HandlerType.Both && t.IsPublic))
            {
                Handler = DynamicObject.CreateDbObjectHandler(t, this);
            }
            else
            {
                Handler = new ReflectionDbObjectHandler(t, this);
            }
        }

        private void InitBySimpleMode(Type t)
        {
            DbObjectHelper.InitObjectInfoBySimpleMode(t, this);
        }

        #endregion

        public IDbObjectHandler Handler;
        internal QueryComposer Composer;

        public string SoftDeleteColumnName;

        public Dictionary<string, List<ASC>> Indexes = new Dictionary<string, List<ASC>>();
        public Dictionary<string, List<MemberHandler>> UniqueIndexes = new Dictionary<string, List<MemberHandler>>();

        public Type BaseType;
        public Type HandleType;
		public FromClause From;
        public string DeleteToTableName;
        public bool HasSystemKey;
        public bool HasAssociate;
        public bool IsAssociateObject;
		public MemberHandler[] KeyFields;
		public MemberHandler[] Fields;
        public MemberHandler[] SimpleFields;
        public MemberHandler[] RelationFields;
        public MemberHandler LockVersion;
        public bool AllowSqlLog = true;
        public bool HasOnePremarykey;
        public Dictionary<Type, ManyToManyMediTable> ManyToManys = new Dictionary<Type,ManyToManyMediTable>();

        internal ObjectInfo() { }

        internal ObjectInfo(Type HandleType, FromClause From, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
        {
            Init(HandleType, From, KeyFields, Fields, DisableSqlLog);
        }

        internal void Init(Type HandleType, FromClause From, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
        {
            this.HandleType = HandleType;
            this.From = From;
            this.KeyFields = KeyFields;
            this.Fields = Fields;
            this.AllowSqlLog = !DisableSqlLog;

            this.HasSystemKey = ((KeyFields.Length == 1) && (KeyFields[0].IsDbGenerate || KeyFields[0].FieldType == typeof(Guid)));

            foreach (MemberHandler f in Fields)
            {
                if (f.IsHasOne || f.IsHasMany || f.IsHasAndBelongsToMany)
                {
                    HasAssociate = true;
                    IsAssociateObject = true;
                }
                if (f.IsLazyLoad)
                {
                    IsAssociateObject = true;
                }
                if (f.IsBelongsTo || f.IsHasAndBelongsToMany) // TODO: no problem ?
                {
                    IsAssociateObject = true;
                }
                if (f.IsLockVersion)
                {
                    LockVersion = f;
                }
            }

            HasOnePremarykey = (KeyFields != null && KeyFields.Length == 1);
        }

        public object NewObject()
        {
            return Handler.CreateInstance();
        }

        internal MemberHandler GetBelongsTo(Type t)
        {
            Type mt = t.IsAbstract ? DynamicObject.GetImplType(t) : t;
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsBelongsTo)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st.IsAbstract)
                    {
                        st = DynamicObject.GetImplType(st);
                    }
                    if (st == mt)
                    {
                        return mh;
                    }
                }
            }
            return null;
            //throw new DbEntryException("Can't find belongs to field of type {0}", t);
        }

        internal MemberHandler GetHasAndBelongsToMany(Type t)
        {
            Type mt = t.IsAbstract ? DynamicObject.GetImplType(t) : t;
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsHasAndBelongsToMany)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st.IsAbstract)
                    {
                        st = DynamicObject.GetImplType(st);
                    }
                    if (st == mt)
                    {
                        return mh;
                    }
                }
            }
            return null;
            //throw new DbEntryException("Can't find belongs to field of type {0}", t);
        }

        public object GetPrimaryKeyDefaultValue()
        {
            if (KeyFields.Length > 1)
            {
                throw new DataException("GetPrimaryKeyDefaultValue don't support multi key.");
            }
            return CommonHelper.GetEmptyValue(KeyFields[0].FieldType, false, "only supported int long guid as primary key.");
        }

        public void LogSql(SqlStatement Sql)
        {
            if (AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
        }

        public bool IsNewObject(object obj)
        {
            return KeyFields[0].UnsavedValue.Equals(Handler.GetKeyValue(obj));
        }

        public static object CloneObject(object obj)
        {
            if (obj == null) { return null; }
            ObjectInfo oi = ObjectInfo.GetInstance(obj.GetType());
            object o = oi.NewObject();
            if (o is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)o).m_InternalInit = true;
            }
            foreach (MemberHandler m in oi.SimpleFields)
            {
                object v = m.GetValue(obj);
                m.SetValue(o, v);
            }
            if (o is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)o).m_InternalInit = false;
            }
            return o;
        }
    }
}
