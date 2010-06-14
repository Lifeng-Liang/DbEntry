using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;
using Lephone.Core;
using Lephone.Core.Text;

namespace Lephone.Data.Common
{
    public partial class ObjectInfo
    {
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

        protected ObjectInfo(Type t, Type baseType)
            : this(t)
        {
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
            if (DataSettings.ObjectHandlerType == HandlerType.Emit
                || (DataSettings.ObjectHandlerType == HandlerType.Both && t.IsPublic))
            {
                _handler = AssemblyHandler.Instance.CreateDbObjectHandler(t, this);
            }
            else
            {
                _handler = new ReflectionDbObjectHandler(t, this);
            }
            // get create tables
            if (From.PartOf != null)
            {
                _createTables = new[] { From.PartOf };
            }
            else if (From.JoinClauseList != null)
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

            if (_baseType == null)
            {
                _baseType = baseType;
                if (ClassHelper.HasAttribute<CacheableAttribute>(baseType, false))
                {
                    _cacheable = true;
                }
            }
            InitContext();
        }

        internal ObjectInfo(Type t)
        {
            var lt = new List<Type>(t.GetInterfaces());
            if (!lt.Contains(typeof(IDbObject)))
            {
                throw new DataException("The data object must implements IDbObject!");
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
            foreach (PropertyInfo pi in t.GetProperties(ClassHelper.InstancePublic))
            {
                if (pi.CanRead && pi.CanWrite)
                {
                    MemberAdapter m = MemberAdapter.NewObject(pi);
                    ProcessMember(m, ret, kfs);
                }
            }
            if (kfs.Count > 1)
            {
                foreach (MemberHandler k in kfs)
                {
                    if (k.IsDbGenerate)
                    {
                        throw new DataException("Multiple key do not allow SystemGeneration!");
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
            SetManyToManyFrom(this, From.MainTableName, Fields);

            _RelationFields = rlfs.ToArray();
            _SimpleFields = sifs.ToArray();

            var sd = ClassHelper.GetAttribute<SoftDeleteAttribute>(t, true);
            if (sd != null)
            {
                _SoftDeleteColumnName = sd.ColumnName;
            }
            var dta = ClassHelper.GetAttribute<DeleteToAttribute>(t, true);
            if (dta != null)
            {
                _DeleteToTableName = dta.TableName;
            }

            GetIndexes();
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

        private static void SetManyToManyFrom(ObjectInfo oi, string mainTableName, IEnumerable<MemberHandler> fields)
        {
            foreach (MemberHandler f in fields)
            {
                if (f.IsHasAndBelongsToMany)
                {
                    Type ft = f.FieldType.GetGenericArguments()[0];
                    string slaveTableName = GetObjectFromClause(ft).MainTableName;

                    string unmappedMainTableName = NameMapper.Instance.UnmapName(mainTableName);
                    string unmappedSlaveTableName = NameMapper.Instance.UnmapName(slaveTableName);

                    string crossTableName = GetCrossTableName(f, unmappedMainTableName, unmappedSlaveTableName);

                    var fc = new FromClause(
                        new JoinClause(crossTableName, unmappedSlaveTableName + "_Id", slaveTableName, "Id",
                                       CompareOpration.Equal, JoinMode.Inner));
                    Type t2 = f.FieldType.GetGenericArguments()[0];
                    oi.CrossTables[t2]
                        = new CrossTable(t2, fc, crossTableName, unmappedMainTableName + "_Id",
                                         unmappedSlaveTableName + "_Id");
                }
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

        private static bool DisableSqlLog(Type dbObjectType)
        {
            object[] ds = dbObjectType.GetCustomAttributes(typeof(DisableSqlLogAttribute), false);
            if (ds.Length != 0)
            {
                return true;
            }
            return false;
        }

    }
}
