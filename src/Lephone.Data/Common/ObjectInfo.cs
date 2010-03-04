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

        public new static ObjectInfo GetInstance(Type dbObjectType)
        {
            if (dbObjectType.IsNotPublic)
            {
                throw new DataException("The model class should be public");
            }
            Type t = (dbObjectType.IsAbstract) ? AssemblyHandler.Instance.GetImplType(dbObjectType) : dbObjectType;
            // should only one TKey here, so use "new" keyword here
            ObjectInfo oi = FlyweightBase<Type, ObjectInfo>.GetInstance(t);
            if (oi.BaseType == null)
            {
                oi._baseType = dbObjectType;
                if (ClassHelper.HasAttribute<CacheableAttribute>(dbObjectType, false))
                {
                    oi._cacheable = true;
                }
            }
            oi.InitContext();
            return oi;
        }

        internal static ObjectInfo GetSimpleInstance(Type dbObjectType)
        {
            Type t = (dbObjectType.IsAbstract) ? AssemblyHandler.Instance.GetImplType(dbObjectType) : dbObjectType;
            if (dic.ContainsKey(t))
            {
                return dic[t];
            }
            var oi = new ObjectInfo();
            oi.InitObjectInfoBySimpleMode(t);
            return oi;
        }

        protected override void Init(Type t)
        {
            var c = ClassHelper.GetArgumentlessConstructor(t);
            if(c == null)
            {
                string typeName = t.Name;
                if(typeName.StartsWith(MemoryTypeBuilder.MemberPrifix))
                {
                    typeName = t.BaseType.Name;
                }
                throw new DataException("class {0} need a public/protected(DbObjectModel) argumentless constructor", typeName);
            }

            InitObjectInfoBySimpleMode(t);
            // binding QueryComposer
            if (!string.IsNullOrEmpty(SoftDeleteColumnName))
            {
                _composer = new SoftDeleteQueryComposer(this, SoftDeleteColumnName);
            }
            else if (!string.IsNullOrEmpty(DeleteToTableName))
            {
                _composer = new DeleteToQueryComposer(this);
            }
            else if (LockVersion != null)
            {
                _composer = new OptimisticLockingQueryComposer(this);
            }
            else
            {
                _composer = new QueryComposer(this);
            }
            // binding DbObjectHandler
            if (DataSetting.ObjectHandlerType == HandlerType.Emit
                || (DataSetting.ObjectHandlerType == HandlerType.Both && t.IsPublic))
            {
                _handler =  AssemblyHandler.Instance.CreateDbObjectHandler(t, this);
            }
            else
            {
                _handler = new ReflectionDbObjectHandler(t, this);
            }
            // get create tables
            if(From.PartOf != null)
            {
                _createTables = new [] { From.PartOf };
            }
            else if(From.JoinClauseList != null)
            {
                var tables = new Dictionary<Type, int>();
                foreach (var joinClause in From.JoinClauseList)
                {
                    if (joinClause.Type1 != null)
                    {
                        tables[joinClause.Type1] = 1;
                    }
                    if (joinClause.Type2 != null)
                    {
                        tables[joinClause.Type2] = 1;
                    }
                }
                if (tables.Count > 0)
                {
                    _createTables = new List<Type>(tables.Keys).ToArray();
                }
            }
        }

        #endregion

        #region properties

        private IDbObjectHandler _handler;
        private QueryComposer _composer;

        private bool _cacheable;
        private Type _baseType;
        private Type _handleType;
        private FromClause _from;
        private bool _hasSystemKey;
        private bool _hasAssociate;
        private bool _isAssociateObject;
        private bool _allowSqlLog = true;
        private bool _hasOnePrimaryKey;
        internal string _DeleteToTableName;
        internal string _SoftDeleteColumnName;
        private MemberHandler _lockVersion;
        private MemberHandler[] _keyFields;
        private MemberHandler[] _fields;
        internal MemberHandler[] _SimpleFields;
        internal MemberHandler[] _RelationFields;

        private readonly Dictionary<string, List<ASC>> _indexes = new Dictionary<string, List<ASC>>();
        private readonly Dictionary<string, List<MemberHandler>> _uniqueIndexes = new Dictionary<string, List<MemberHandler>>();
        private readonly Dictionary<Type, CrossTable> _crossTables = new Dictionary<Type, CrossTable>();

        private DbContext _context;

        private Type[] _createTables;

        public IDbObjectHandler Handler
        {
            get { return _handler; }
        }

        internal QueryComposer Composer
        {
            get { return _composer; }
        }

        public bool Cacheable
        {
            get { return _cacheable; }
        }

        public Type BaseType
        {
            get { return _baseType; }
        }

        public Type HandleType
        {
            get { return _handleType; }
        }

        public FromClause From
        {
            get { return _from; }
        }

        public bool HasSystemKey
        {
            get { return _hasSystemKey; }
        }

        public bool HasAssociate
        {
            get { return _hasAssociate; }
        }

        public bool IsAssociateObject
        {
            get { return _isAssociateObject; }
        }

        public bool AllowSqlLog
        {
            get { return _allowSqlLog; }
        }

        public bool HasOnePrimaryKey
        {
            get { return _hasOnePrimaryKey; }
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
            get { return _lockVersion; }
        }

        public MemberHandler[] KeyFields
        {
            get { return _keyFields; }
        }

        public MemberHandler[] Fields
        {
            get { return _fields; }
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
            get { return _indexes; }
        }

        public Dictionary<string, List<MemberHandler>> UniqueIndexes
        {
            get { return _uniqueIndexes; }
        }

        public Dictionary<Type, CrossTable> CrossTables
        {
            get { return _crossTables; }
        }

        public Type[] CreateTables
        {
            get { return _createTables; }
        }

        public DbContext Context
        {
            get { return _context; }
        }

        #endregion

        #region ctor

        protected ObjectInfo() { }

        internal ObjectInfo(Type handleType, FromClause from, MemberHandler[] keyFields, MemberHandler[] fields, bool disableSqlLog)
        {
            Init(handleType, from, keyFields, fields, disableSqlLog);
        }

        internal void InitContext()
        {
            var attr = ClassHelper.GetAttribute<DbContextAttribute>(_baseType, true);
            _context = DbEntry.GetContext(attr == null ? null : attr.ContextName);
        }

        internal void Init(Type handleType, FromClause fromClause, MemberHandler[] keyFields, MemberHandler[] fields, bool disableSqlLog)
        {
            _handleType = handleType;
            _from = fromClause;
            _keyFields = keyFields;
            _fields = fields;
            _allowSqlLog = !disableSqlLog;

            _hasSystemKey = ((keyFields != null && keyFields.Length == 1) && (keyFields[0].IsDbGenerate || keyFields[0].FieldType == typeof(Guid)));

            foreach (MemberHandler f in fields)
            {
                if (f.IsHasOne || f.IsHasMany || f.IsHasAndBelongsToMany)
                {
                    _hasAssociate = true;
                    _isAssociateObject = true;
                }
                if (f.IsLazyLoad)
                {
                    _isAssociateObject = true;
                }
                if (f.IsBelongsTo || f.IsHasAndBelongsToMany) // TODO: no problem ?
                {
                    _isAssociateObject = true;
                }
                if (f.IsLockVersion)
                {
                    _lockVersion = f;
                }
            }

            _hasOnePrimaryKey = (keyFields != null && keyFields.Length == 1);
        }

        #endregion

        #region shortcut functions

        public object NewObject()
        {
            return Handler.CreateInstance();
        }

        internal MemberHandler GetBelongsTo(Type t)
        {
            Type mt = t.IsAbstract ? AssemblyHandler.Instance.GetImplType(t) : t;
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsBelongsTo)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st.IsAbstract)
                    {
                        st = AssemblyHandler.Instance.GetImplType(st);
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
            Type mt = t.IsAbstract ? AssemblyHandler.Instance.GetImplType(t) : t;
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsHasAndBelongsToMany)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st.IsAbstract)
                    {
                        st = AssemblyHandler.Instance.GetImplType(st);
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

        public void LogSql(SqlStatement sql)
        {
            if (AllowSqlLog)
            {
                Logger.SQL.Trace(sql);
            }
        }

        public bool IsNewObject(object obj)
        {
            return KeyFields[0].UnsavedValue.Equals(Handler.GetKeyValue(obj));
        }

        public static object CloneObject(object obj, DbContext context)
        {
            if (obj == null) { return null; }
            ObjectInfo oi = GetInstance(obj.GetType());
            object o = oi.NewObject();
            if (o is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)o).m_InternalInit = true;
            }
            if (context != null)
            {
                foreach (MemberHandler mh in oi.RelationFields)
                {
                    if (mh.IsBelongsTo || mh.IsHasAndBelongsToMany)
                    {
                        var bt = (ILazyLoading)mh.GetValue(o);
                        bt.Init(context, mh.Name);
                    }
                }
            }
            foreach (MemberHandler m in oi.SimpleFields)
            {
                object v = m.GetValue(obj);
                m.SetValue(o, v);
            }
            foreach (var m in oi.RelationFields)
            {
                if (m.IsBelongsTo)
                {
                    var os = (IBelongsTo) m.GetValue(obj);
                    var od = (IBelongsTo) m.GetValue(o);
                    od.ForeignKey = os.ForeignKey;
                }
            }
            Doit(context, o, oi);
            if (o is DbObjectSmartUpdate)
            {
                ((DbObjectSmartUpdate)o).m_InternalInit = false;
            }
            return o;
        }

        public static object CloneObject(object obj)
        {
            return CloneObject(obj, null);
        }

        // TODO: move this method to IObjectHandler
        // TODO: Or IObjectHandler should have a method called "CloneObject"?
        private static void Doit(DbContext driver, object o, ObjectInfo oi)
        {
            foreach (MemberHandler f in oi.RelationFields)
            {
                var ho = (ILazyLoading)f.GetValue(o);
                if (f.IsLazyLoad)
                {
                    ho.Init(driver, f.Name);
                }
                else if (f.IsHasOne || f.IsHasMany)
                {
                    ObjectInfo oi1 = GetInstance(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetBelongsTo(oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(driver, h1.Name);
                    }
                    else
                    {
                        // TODO: should throw exception or not ?
                        throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
                        // ho.Init(driver, "__");
                    }
                }
                else if (f.IsHasAndBelongsToMany)
                {
                    ObjectInfo oi1 = GetInstance(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetHasAndBelongsToMany(oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(driver, h1.Name);
                    }
                    else
                    {
                        // TODO: should throw exception or not ?
                        throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
                        // ho.Init(driver, "__");
                    }
                }
            }
        }

        #endregion
    }
}
