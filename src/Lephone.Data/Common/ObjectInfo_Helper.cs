using System;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using Lephone.Util;
using Lephone.Util.Setting;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;
using Lephone.Util.Text;

namespace Lephone.Data.Common
{
    public partial class ObjectInfo
    {
        internal void InitObjectInfoBySimpleMode(Type t)
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
                if (mh.IsHasOne || mh.IsHasMany || mh.IsHasAndBelongsToMany || mh.IsBelongsTo || mh.IsLazyLoad)
                {
                    rlfs.Add(mh);
                }
                else
                {
                    sifs.Add(mh);
                }
            }
            var fields = new List<MemberHandler>(sifs);
            fields.AddRange(rlfs);
            MemberHandler[] keys = kfs.ToArray();

            this.Init(t, GetObjectFromClause(t), keys, fields.ToArray(), DisableSqlLog(t));
            SetManyToManyMediFrom(this, this.From.GetMainTableName(), this.Fields);

            this._RelationFields = rlfs.ToArray();
            this._SimpleFields = sifs.ToArray();

            var sd = ClassHelper.GetAttribute<SoftDeleteAttribute>(t, true);
            if (sd != null)
            {
                this._SoftDeleteColumnName = sd.ColumnName;
            }
            var dta = ClassHelper.GetAttribute<DeleteToAttribute>(t, true);
            if (dta != null)
            {
                this._DeleteToTableName = dta.TableName;
            }

            this.GetIndexes();
        }

        public static object CreateObject(DbContext context, Type DbObjectType, IDataReader dr, bool UseIndex)
        {
            ObjectInfo oi = GetInstance(DbObjectType);
            object obj = oi.NewObject();
            var sudi = obj as DbObjectSmartUpdate;
            if (sudi != null)
            {
                sudi.m_InternalInit = true;
            }
            foreach (MemberHandler mh in oi.RelationFields)
            {
                if (mh.IsBelongsTo || mh.IsHasAndBelongsToMany)
                {
                    var bt = (ILazyLoading)mh.GetValue(obj);
                    bt.Init(context, mh.Name);
                }
            }
            oi.Handler.LoadSimpleValues(obj, UseIndex, dr);
            oi.Handler.LoadRelationValues(context, obj, UseIndex, dr);
            if (sudi != null)
            {
                sudi.m_InternalInit = false;
            }
            return obj;
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
                        throw new ApplicationException("Cann't set the same name index more than ones at the same column.");
                    }
                }
                ls.Add(ia.IndexName);
            }
        }

        public static FromClause GetFromClause(Type DbObjectType)
        {
            ObjectInfo oi = GetInstance(DbObjectType);
            return oi.From;
        }

        public static WhereCondition GetKeyWhereClause(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = GetInstance(t);
            if (oi.KeyFields == null)
            {
                throw new DataException("dbobject not define key field : " + t);
            }
            WhereCondition ret = null;
            Dictionary<string,object> dictionary = oi.Handler.GetKeyValues(obj);
            foreach (string s in dictionary.Keys)
            {
                ret &= (CK.K[s] == dictionary[s]);
            }
            return ret;
        }

        public static void SetKey(object obj, object key)
        {
            Type t = obj.GetType();
            ObjectInfo oi = GetInstance(t);
            if (!oi.HasSystemKey)
            {
                throw new DataException("dbobject not define SystemGeneration key field : " + t);
            }
            MemberHandler fh = oi.KeyFields[0];
            object sKey;
            if (fh.FieldType == typeof(long))
            {
                sKey = Convert.ToInt64(key);
            }
            else if (fh.FieldType == typeof(int))
            {
                sKey = Convert.ToInt32(key);
            }
            else
            {
                sKey = key;
            }
            fh.SetValue(obj, sKey);
        }

        public static string GetColumuName(MemberAdapter fi)
        {
            var fn = fi.GetAttribute<DbColumnAttribute>(false);
            return (fn == null) ? fi.Name : fn.Name;
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

        private void GetIndexes()
        {
            foreach (MemberHandler fh in this.Fields)
            {
                IndexAttribute[] ias = fh.MemberInfo.GetAttributes<IndexAttribute>(false);
                CheckIndexAttributes(ias);
                foreach (IndexAttribute ia in ias)
                {
                    ASC a = ia.ASC ? (ASC)fh.Name : (DESC)fh.Name;
                    string key = ia.IndexName ?? a.Key;
                    if (!this.Indexes.ContainsKey(key))
                    {
                        this.Indexes.Add(key, new List<ASC>());
                    }
                    this.Indexes[key].Add(a);
                    if (ia.UNIQUE)
                    {
                        if (!this.UniqueIndexes.ContainsKey(key))
                        {
                            this.UniqueIndexes.Add(key, new List<MemberHandler>());
                        }
                        this.UniqueIndexes[key].Add(fh);
                    }
                }
            }
        }

        internal static MemberHandler GetKeyField(Type tt)
        {
            ObjectInfo oi = GetSimpleInstance(tt);
            if (oi.KeyFields.Length > 0)
            {
                return oi.KeyFields[0];
            }
            return null;
        }

        private static void SetManyToManyMediFrom(ObjectInfo oi, string MainTableName, IEnumerable<MemberHandler> Fields)
        {
            foreach (MemberHandler f in Fields)
            {
                if (f.IsHasAndBelongsToMany)
                {
                    Type ft = f.FieldType.GetGenericArguments()[0];
                    string SlaveTableName = GetObjectFromClause(ft).GetMainTableName();

                    string UnmappedMainTableName = NameMapper.Instance.UnmapName(MainTableName);
                    string UnmappedSlaveTableName = NameMapper.Instance.UnmapName(SlaveTableName);

                    string MediTableName = UnmappedMainTableName.CompareTo(UnmappedSlaveTableName) > 0 ?
                        UnmappedSlaveTableName + "_" + UnmappedMainTableName : UnmappedMainTableName + "_" + UnmappedSlaveTableName;
                    MediTableName = NameMapper.Instance.Prefix + MediTableName;

                    var fc = new FromClause(
                        new JoinClause(MediTableName + "." + UnmappedSlaveTableName + "_Id", SlaveTableName + ".Id",
                            CompareOpration.Equal, JoinMode.Inner));
                    Type t2 = f.FieldType.GetGenericArguments()[0];
                    oi.ManyToManys[t2]
                        = new ManyToManyMediTable(t2, fc, MediTableName, UnmappedMainTableName + "_Id", UnmappedSlaveTableName + "_Id");
                }
            }
        }

        private static bool DisableSqlLog(Type DbObjectType)
        {
            object[] ds = DbObjectType.GetCustomAttributes(typeof(DisableSqlLogAttribute), false);
            if (ds.Length != 0)
            {
                return true;
            }
            return false;
        }

        internal static FromClause GetObjectFromClause(Type DbObjectType)
        {
            var dtas = (DbTableAttribute[])DbObjectType.GetCustomAttributes(typeof(DbTableAttribute), false);
            var joas = (JoinOnAttribute[])DbObjectType.GetCustomAttributes(typeof(JoinOnAttribute), false);
            if (dtas.Length != 0 && joas.Length != 0)
            {
                throw new ArgumentException(string.Format("class [{0}] defined DbTable and JoinOn. Only one allowed.", DbObjectType.Name));
            }
            if (dtas.Length == 0)
            {
                if (joas.Length == 0)
                {
                    string DefaultName = NameMapper.Instance.MapName(DbObjectType.Name);
                    return new FromClause(GetTableNameFromConfig(DefaultName));
                }
                var jcs = new JoinClause[joas.Length];
                for (int i = 0; i < joas.Length; i++)
                {
                    int n = joas[i].Index;
                    if (n < 0)
                    {
                        n = i;
                    }
                    jcs[n] = joas[i].joinner;
                }
                foreach (JoinClause jc in jcs)
                {
                    if (jc == null)
                    {
                        throw new ArgumentException(string.Format("class [{0}] JoinOnAttribute defined error.", DbObjectType.Name));
                    }
                }
                return new FromClause(jcs);
            }
            if (dtas[0].TableName != null)
            {
                return new FromClause(GetTableNameFromConfig(dtas[0].TableName));
            }
            return new FromClause(dtas[0].LinkNames);
        }

        internal static string GetTableNameFromConfig(string DefinedName)
        {
            return ConfigHelper.DefaultSettings.GetValue("@" + DefinedName, DefinedName);
        }
    }
}
