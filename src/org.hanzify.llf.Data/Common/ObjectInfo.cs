using System;
using System.Collections.Generic;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;
using Lephone.Data.Builder.Clause;
using Lephone.Util.Logging;
using Lephone.Util;

namespace Lephone.Data.Common
{
    public partial class ObjectInfo : FlyweightBase<Type, ObjectInfo>
    {
        #region GetInstance

        public new static ObjectInfo GetInstance(Type DbObjectType)
        {
            Type t = (DbObjectType.IsAbstract) ? DynamicObject.GetImplType(DbObjectType) : DbObjectType;
            ObjectInfo oi = FlyweightBase<Type, ObjectInfo>.GetInstance(t);
            if (oi.BaseType == null)
            {
                oi._BaseType = DbObjectType;
                if (ClassHelper.HasAttribute<CacheableAttribute>(DbObjectType, false))
                {
                    oi._Cacheable = true;
                }
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
            ObjectInfo oi = new ObjectInfo();
            oi.InitBySimpleMode(t);
            return oi;
        }

        protected override void Init(Type t)
        {
            InitBySimpleMode(t);
            // binding QueryComposer
            if (!string.IsNullOrEmpty(SoftDeleteColumnName))
            {
                _Composer = new SoftDeleteQueryComposer(this, SoftDeleteColumnName);
            }
            else if (!string.IsNullOrEmpty(DeleteToTableName))
            {
                _Composer = new DeleteToQueryComposer(this);
            }
            else if (LockVersion != null)
            {
                _Composer = new OptimisticLockingQueryComposer(this);
            }
            else
            {
                _Composer = new QueryComposer(this);
            }
            // binding DbObjectHandler
            if (DataSetting.ObjectHandlerType == HandlerType.Emit
                || (DataSetting.ObjectHandlerType == HandlerType.Both && t.IsPublic))
            {
                _Handler = DynamicObject.CreateDbObjectHandler(t, this);
            }
            else
            {
                _Handler = new ReflectionDbObjectHandler(t, this);
            }
        }

        private void InitBySimpleMode(Type t)
        {
            this.InitObjectInfoBySimpleMode(t);
        }

        #endregion

        #region properties

        private IDbObjectHandler _Handler;
        private QueryComposer _Composer;

        private bool _Cacheable;
        private Type _BaseType;
        private Type _HandleType;
        private FromClause _From;
        private bool _HasSystemKey;
        private bool _HasAssociate;
        private bool _IsAssociateObject;
        private bool _AllowSqlLog = true;
        private bool _HasOnePremarykey;
        internal string _DeleteToTableName;
        internal string _SoftDeleteColumnName;
        private MemberHandler _LockVersion;
        private MemberHandler[] _KeyFields;
        private MemberHandler[] _Fields;
        internal MemberHandler[] _SimpleFields;
        internal MemberHandler[] _RelationFields;

        private readonly Dictionary<string, List<ASC>> _Indexes = new Dictionary<string, List<ASC>>();
        private readonly Dictionary<string, List<MemberHandler>> _UniqueIndexes = new Dictionary<string, List<MemberHandler>>();
        private readonly Dictionary<Type, ManyToManyMediTable> _ManyToManys = new Dictionary<Type, ManyToManyMediTable>();

        public IDbObjectHandler Handler
        {
            get { return _Handler; }
        }

        internal QueryComposer Composer
        {
            get { return _Composer; }
        }

        public bool Cacheable
        {
            get { return _Cacheable; }
        }

        public Type BaseType
        {
            get { return _BaseType; }
        }

        public Type HandleType
        {
            get { return _HandleType; }
        }

        public FromClause From
        {
            get { return _From; }
        }

        public bool HasSystemKey
        {
            get { return _HasSystemKey; }
        }

        public bool HasAssociate
        {
            get { return _HasAssociate; }
        }

        public bool IsAssociateObject
        {
            get { return _IsAssociateObject; }
        }

        public bool AllowSqlLog
        {
            get { return _AllowSqlLog; }
        }

        public bool HasOnePremarykey
        {
            get { return _HasOnePremarykey; }
        }

        public string DeleteToTableName
        {
            get { return _DeleteToTableName; }
        }

        public string SoftDeleteColumnName
        {
            get { return _SoftDeleteColumnName; }
        }

        public MemberHandler LockVersion
        {
            get { return _LockVersion; }
        }

        public MemberHandler[] KeyFields
        {
            get { return _KeyFields; }
        }

        public MemberHandler[] Fields
        {
            get { return _Fields; }
        }

        public MemberHandler[] SimpleFields
        {
            get { return _SimpleFields; }
        }

        public MemberHandler[] RelationFields
        {
            get { return _RelationFields; }
        }

        public Dictionary<string, List<ASC>> Indexes
        {
            get { return _Indexes; }
        }

        public Dictionary<string, List<MemberHandler>> UniqueIndexes
        {
            get { return _UniqueIndexes; }
        }

        public Dictionary<Type, ManyToManyMediTable> ManyToManys
        {
            get { return _ManyToManys; }
        }

        #endregion

        #region ctor

        internal ObjectInfo() { }

        internal ObjectInfo(Type HandleType, FromClause From, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
        {
            Init(HandleType, From, KeyFields, Fields, DisableSqlLog);
        }

        internal void Init(Type HandleType, FromClause From, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
        {
            this._HandleType = HandleType;
            this._From = From;
            this._KeyFields = KeyFields;
            this._Fields = Fields;
            this._AllowSqlLog = !DisableSqlLog;

            this._HasSystemKey = ((KeyFields.Length == 1) && (KeyFields[0].IsDbGenerate || KeyFields[0].FieldType == typeof(Guid)));

            foreach (MemberHandler f in Fields)
            {
                if (f.IsHasOne || f.IsHasMany || f.IsHasAndBelongsToMany)
                {
                    _HasAssociate = true;
                    _IsAssociateObject = true;
                }
                if (f.IsLazyLoad)
                {
                    _IsAssociateObject = true;
                }
                if (f.IsBelongsTo || f.IsHasAndBelongsToMany) // TODO: no problem ?
                {
                    _IsAssociateObject = true;
                }
                if (f.IsLockVersion)
                {
                    _LockVersion = f;
                }
            }

            _HasOnePremarykey = (KeyFields != null && KeyFields.Length == 1);
        }

        #endregion

        #region shortcut functions

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
            ObjectInfo oi = GetInstance(obj.GetType());
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

        #endregion
    }
}
