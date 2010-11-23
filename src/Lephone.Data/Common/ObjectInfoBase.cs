using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Core;
using Lephone.Core.Setting;
using Lephone.Core.Text;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public class ObjectInfoBase
    {
        #region properties

        private IDbObjectHandler _handler;

        private Type _handleType;
        private FromClause _from;
        private bool _hasSystemKey;
        private bool _hasAssociate;
        private bool _isAssociateObject;
        private bool _allowSqlLog = true;
        private bool _hasOnePrimaryKey;
        private MemberHandler _lockVersion;
        private MemberHandler[] _keyFields;
        private MemberHandler[] _fields;

        private readonly Dictionary<string, List<ASC>> _indexes = new Dictionary<string, List<ASC>>();
        private readonly Dictionary<string, List<MemberHandler>> _uniqueIndexes = new Dictionary<string, List<MemberHandler>>();
        private readonly Dictionary<Type, CrossTable> _crossTables = new Dictionary<Type, CrossTable>();

        private DbContext _context;

        public readonly string DeleteToTableName;
        public readonly string SoftDeleteColumnName;
        public readonly MemberHandler[] SimpleFields;
        public readonly MemberHandler[] RelationFields;

        public IDbObjectHandler Handler
        {
            get
            {
                if (_handler == null)
                {
                    lock (this)
                    {
                        if (_handler == null)
                        {
                            _handler = CreateDbObjectHandler(_handleType);
                        }
                    }
                }
                return _handler;
            }
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

        public DbContext Context
        {
            get
            {
                if (_context == null)
                {
                    lock (this)
                    {
                        if (_context == null)
                        {
                            InitContext();
                        }
                    }
                }
                return _context;
            }
        }

        #endregion

        protected ObjectInfoBase()
        {
        }

        internal ObjectInfoBase(Type t)
        {
            var lt = new List<Type>(t.GetInterfaces());
            if (!lt.Contains(typeof(IDbObject)))
            {
                throw new ModelException(t, "The data object must implements IDbObject!");
            }

            var ret = new List<MemberHandler>();
            var kfs = new List<MemberHandler>();
            foreach (FieldInfo fi in t.GetFields(ClassHelper.InstanceFlag))
            {
                if (!fi.IsPrivate)
                {
                    MemberAdapter m = MemberAdapter.NewObject(fi);
                    ProcessMember(m, ret, kfs);
                }
            }
            foreach (PropertyInfo pi in t.GetProperties(ClassHelper.InstanceFlag))
            {
                if (pi.CanRead && pi.CanWrite && pi.GetGetMethod(true) != null && !pi.GetGetMethod(true).IsPrivate)
                {
                    var mi = pi.GetGetMethod(true);
                    if(mi != null && !mi.IsPrivate)
                    {
                        MemberAdapter m = MemberAdapter.NewObject(pi);
                        ProcessMember(m, ret, kfs);
                    }
                }
            }
            if (kfs.Count > 1)
            {
                foreach (MemberHandler k in kfs)
                {
                    if (k.IsDbGenerate)
                    {
                        throw new ModelException(k.MemberInfo, "Multiple key do not allow to be SystemGeneration!");
                    }
                }
            }

            // fill simple and relation fields.
            var rlfs = new List<MemberHandler>();
            var sifs = new List<MemberHandler>();
            foreach (MemberHandler mh in ret)
            {
                if (mh.IsSimpleField)
                {
                    sifs.Add(mh);
                }
                else
                {
                    rlfs.Add(mh);
                }
            }
            var fields = new List<MemberHandler>(sifs);
            fields.AddRange(rlfs);
            MemberHandler[] keys = kfs.ToArray();

            Init(t, GetObjectFromClause(t), keys, fields.ToArray(), DisableSqlLog(t));
            SetManyToManyFrom(this, From.MainOriginTableName, Fields);

            RelationFields = rlfs.ToArray();
            SimpleFields = sifs.ToArray();

            var sd = ClassHelper.GetAttribute<SoftDeleteAttribute>(t, true);
            if (sd != null)
            {
                SoftDeleteColumnName = sd.ColumnName;
            }
            var dta = ClassHelper.GetAttribute<DeleteToAttribute>(t, true);
            if (dta != null)
            {
                DeleteToTableName = dta.TableName;
            }

            GetIndexes();
        }

        private static void ProcessMember(MemberAdapter m, IList<MemberHandler> ret, ICollection<MemberHandler> kfs)
        {
            if (!(
                     m.HasAttribute<ExcludeAttribute>(false) ||
                     m.HasAttribute<HasOneAttribute>(false) ||
                     m.HasAttribute<HasManyAttribute>(false) ||
                     m.HasAttribute<HasAndBelongsToManyAttribute>(false) ||
                     m.HasAttribute<BelongsToAttribute>(false) ||
                     m.HasAttribute<LazyLoadAttribute>(false)))
            {
                MemberHandler fh = MemberHandler.NewObject(m);
                if (fh.IsKey)
                {
                    kfs.Add(fh);
                    ret.Insert(0, fh);
                }
                else
                {
                    ret.Add(fh);
                }
            }
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
                if (f.IsHasOne || f.IsHasMany || f.IsHasAndBelongsToMany || f.IsBelongsTo)
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

        internal static FromClause GetObjectFromClause(Type dbObjectType)
        {
            var dtas = (DbTableAttribute[])dbObjectType.GetCustomAttributes(typeof(DbTableAttribute), false);
            var joas = (JoinOnAttribute[])dbObjectType.GetCustomAttributes(typeof(JoinOnAttribute), false);
            if (dtas.Length != 0 && joas.Length != 0)
            {
                throw new ArgumentException(string.Format("class [{0}] defined DbTable and JoinOn. Only one allowed.",
                                                          dbObjectType.Name));
            }
            if (dtas.Length == 0)
            {
                if (joas.Length == 0)
                {
                    string defaultName = NameMapper.Instance.MapName(dbObjectType.Name);
                    return new FromClause(dbObjectType.Name, GetTableNameFromConfig(defaultName));
                }
                var jcs = new JoinClause[joas.Length];
                for (int i = 0; i < joas.Length; i++)
                {
                    int n = joas[i].Index;
                    if (n < 0)
                    {
                        n = i;
                    }
                    jcs[n] = joas[i].Joinner;
                }
                foreach (JoinClause jc in jcs)
                {
                    if (jc == null)
                    {
                        throw new ArgumentException(string.Format("class [{0}] JoinOnAttribute defined error.",
                                                                  dbObjectType.Name));
                    }
                }
                return new FromClause(jcs);
            }
            if (dtas[0].TableName == null)
            {
                return new FromClause(dtas[0].PartOf);
            }
            return new FromClause(GetTableNameFromConfig(dtas[0].TableName));
        }

        private static bool DisableSqlLog(Type dbObjectType)
        {
            object[] ds = dbObjectType.GetCustomAttributes(typeof(DisableSqlLogAttribute), false);
            if (ds.Length != 0)
            {
                return true;
            }
            return false;
        }

        private static void SetManyToManyFrom(ObjectInfoBase oi, string unmappedMainTableName, IEnumerable<MemberHandler> fields)
        {
            foreach (MemberHandler f in fields)
            {
                if (f.IsHasAndBelongsToMany)
                {
                    Type ft = f.FieldType.GetGenericArguments()[0];
                    var slave = GetObjectFromClause(ft);
                    string unmappedSlaveTableName = slave.MainOriginTableName;

                    string crossTableName = GetCrossTableName(f, unmappedMainTableName, unmappedSlaveTableName);

                    var fc = new FromClause(
                        new JoinClause(crossTableName, unmappedSlaveTableName + "_Id", slave.MainTableName, "Id",
                                       CompareOpration.Equal, JoinMode.Inner));
                    Type t2 = f.FieldType.GetGenericArguments()[0];
                    oi.CrossTables[t2]
                        = new CrossTable(t2, fc, crossTableName, unmappedMainTableName + "_Id",
                                         unmappedSlaveTableName + "_Id");
                }
            }
        }

        private void GetIndexes()
        {
            foreach (MemberHandler fh in Fields)
            {
                IndexAttribute[] ias = fh.MemberInfo.GetAttributes<IndexAttribute>(false);
                CheckIndexAttributes(ias);
                foreach (IndexAttribute ia in ias)
                {
                    ASC a = ia.ASC ? (ASC)fh.Name : (DESC)fh.Name;
                    string key = ia.IndexName ?? a.Key;
                    if (!Indexes.ContainsKey(key))
                    {
                        Indexes.Add(key, new List<ASC>());
                    }
                    Indexes[key].Add(a);
                    if (ia.UNIQUE)
                    {
                        if (!UniqueIndexes.ContainsKey(key))
                        {
                            UniqueIndexes.Add(key, new List<MemberHandler>());
                        }
                        UniqueIndexes[key].Add(fh);
                    }
                }
            }
        }

        private static void CheckIndexAttributes(IEnumerable<IndexAttribute> ias)
        {
            var ls = new List<string>();
            foreach (IndexAttribute ia in ias)
            {
                foreach (string s in ls)
                {
                    if (ia.IndexName == s)
                    {
                        throw new ApplicationException(
                            "Cann't set the same name index more than ones at the same column.");
                    }
                }
                ls.Add(ia.IndexName);
            }
        }

        private static string GetCrossTableName(MemberHandler f, string unmappedMainTableName, string unmappedSlaveTableName)
        {
            string crossTableName;
            if (!string.IsNullOrEmpty(f.CrossTableName))
            {
                crossTableName = f.CrossTableName;
            }
            else
            {
                crossTableName
                    = unmappedMainTableName.CompareTo(unmappedSlaveTableName) > 0
                          ? unmappedSlaveTableName + "_" + unmappedMainTableName
                          : unmappedMainTableName + "_" + unmappedSlaveTableName;
            }
            crossTableName = NameMapper.Instance.Prefix + crossTableName;
            return crossTableName;
        }

        private static string GetTableNameFromConfig(string definedName)
        {
            return ConfigHelper.DefaultSettings.GetValue("@" + definedName, definedName);
        }

        public IDbObjectHandler CreateDbObjectHandler(Type sourceType)
        {
            if (sourceType.IsGenericType)
            {
                switch (sourceType.Name)
                {
                    case "GroupByObject`1":
                        var t = typeof(GroupbyObjectHandler<>).MakeGenericType(sourceType.GetGenericArguments());
                        return (IDbObjectHandler)ClassHelper.CreateInstance(t);
                    case "GroupBySumObject`2":
                        var ts = typeof(GroupbySumObjectHandler<,>).MakeGenericType(sourceType.GetGenericArguments());
                        return (IDbObjectHandler)ClassHelper.CreateInstance(ts);
                    default:
                        throw new NotSupportedException();
                }
            }
            var attr = ClassHelper.GetAttribute<ModelHandlerAttribute>(sourceType, false);
            if (attr != null)
            {
                var o = (EmitObjectHandlerBase)ClassHelper.CreateInstance(attr.Type);
                o.Init(this);
                return o;
            }
            throw new ModelException(sourceType, "Can not find ObjectHandler.");
        }

        internal void InitContext()
        {
            var attr = ClassHelper.GetAttribute<DbContextAttribute>(_handleType, true);
            _context = DbEntry.GetContext(attr == null ? null : attr.ContextName);
        }

        internal MemberHandler GetBelongsTo(Type t)
        {
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsBelongsTo)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st == t)
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
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsHasAndBelongsToMany)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st == t)
                    {
                        return mh;
                    }
                }
            }
            return null;
            //throw new DbEntryException("Can't find belongs to field of type {0}", t);
        }
    }
}
