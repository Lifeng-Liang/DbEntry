using System;
using System.Collections.Generic;
using Lephone.Data.Builder.Clause;
using Lephone.Util;

namespace Lephone.Data.Common
{
    public partial class ObjectInfo : FlyweightBase<Type, ObjectInfo>
    {
        #region Flyweight GetInstance

        protected override Type CheckKey(Type dbObjectType)
        {
            if (dbObjectType.IsNotPublic)
            {
                throw new DataException("The model class should be public");
            }
            Type t = (dbObjectType.IsAbstract) ? AssemblyHandler.Instance.GetImplType(dbObjectType) : dbObjectType;
            var c = ClassHelper.GetArgumentlessConstructor(t);
            if (c == null)
            {
                string typeName = t.Name;
                if (typeName.StartsWith(MemoryTypeBuilder.MemberPrifix))
                {
                    typeName = t.BaseType.Name;
                }
                throw new DataException("class {0} need a public/protected(DbObjectModel) argumentless constructor", typeName);
            }
            return t;
        }

        protected override ObjectInfo CreateInst(Type t)
        {
            var bt = t.Name.StartsWith("$") ? t.BaseType : t;
            return new ObjectInfo(t, bt);
        }

        internal static ObjectInfo GetSimpleInstance(Type dbObjectType)
        {
            Type t = (dbObjectType.IsAbstract) ? AssemblyHandler.Instance.GetImplType(dbObjectType) : dbObjectType;
            if (Jar.ContainsKey(t))
            {
                return Jar[t];
            }
            var oi = new ObjectInfo(t);
            return oi;
        }

        #endregion

        #region properties

        private IDbObjectHandler _handler;
        private QueryComposer _composer;

        private bool _cacheable;
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

        private Type[] _createTables;

        private Type _baseType;
        internal string _DeleteToTableName;
        internal string _SoftDeleteColumnName;
        internal MemberHandler[] _SimpleFields;
        internal MemberHandler[] _RelationFields;

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
    }
}
