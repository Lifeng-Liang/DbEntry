
#region usings

using System;
using System.Reflection;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using Lephone.Util;
using Lephone.Util.Setting;
using Lephone.Data.Builder;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;
using Lephone.Data.Driver;
using Lephone.Data.Definition;

#endregion

namespace Lephone.Data.Common
{
    public static class DbObjectHelper
    {
        private static HybridDictionary ObjectInfos = new HybridDictionary();

        public static object CreateObject(DbContext context, Type DbObjectType, IDataReader dr, bool UseIndex)
        {
            ObjectInfo oi = m_GetObjectInfo(DbObjectType);
            object obj = oi.NewObject();
            DbObjectSmartUpdate sudi = obj as DbObjectSmartUpdate;
            if (sudi != null)
            {
                sudi.m_InternalInit = true;
            }
            foreach (MemberHandler mh in oi.RelationFields)
            {
                if (mh.IsBelongsTo || mh.IsHasAndBelongsToMany)
                {
                    ILazyLoading bt = (ILazyLoading)mh.GetValue(obj);
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

        private static void CheckIndexAttributes(IndexAttribute[] ias)
        {
            List<string> ls = new List<string>();
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

        public static void FillIndexes(CreateTableStatementBuilder ct, Type t)
        {
            ObjectInfo oi = m_GetObjectInfo(t);
            Dictionary<string, List<ASC>> dict = new Dictionary<string,List<ASC>>();
            Dictionary<string, bool> UniIndex = new Dictionary<string, bool>();
            foreach( MemberHandler fh in oi.Fields)
            {
                IndexAttribute[] ias = (IndexAttribute[])fh.MemberInfo.GetCustomAttributes(typeof(IndexAttribute), false);
                CheckIndexAttributes(ias);
                foreach (IndexAttribute ia in ias)
                {
                    ASC a = ia.ASC ? (ASC)fh.Name : (DESC)fh.Name;
                    if (ia.IndexName == null)
                    {
                        ct.Indexes.Add(new DbIndex(ia.IndexName, ia.UNIQUE, a));
                    }
                    else
                    {
                        if (!dict.ContainsKey(ia.IndexName))
                        {
                            dict.Add(ia.IndexName, new List<ASC>());
                        }
                        dict[ia.IndexName].Add(a);
                        if (ia.UNIQUE)
                        {
                            UniIndex[ia.IndexName] = true;
                        }
                    }
                }
            }
            foreach (string s in dict.Keys)
            {
                bool u = UniIndex.ContainsKey(s) ? true : false;
                ct.Indexes.Add(new DbIndex(s, u, dict[s].ToArray()));
            }
        }

        public static ObjectInfo GetObjectInfo(Type DbObjectType)
        {
            return m_GetObjectInfo(DbObjectType);
        }

        public static FromClause GetFromClause(Type DbObjectType)
        {
            ObjectInfo oi = m_GetObjectInfo(DbObjectType);
            return oi.From;
        }

        public static WhereCondition GetKeyWhereClause(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = m_GetObjectInfo(t);
            if (oi.KeyFields == null)
            {
                throw new DbEntryException("dbobject not define key field : " + t.ToString());
            }
            WhereCondition ret = null;
            Dictionary<string,object> dic = oi.Handler.GetKeyValues(obj);
            foreach (string s in dic.Keys)
            {
                ret &= (CK.K[s] == dic[s]);
            }
            return ret;
        }

        public static void SetKey(object obj, object key)
        {
            Type t = obj.GetType();
            ObjectInfo oi = m_GetObjectInfo(t);
            if (!oi.HasSystemKey)
            {
                throw new DbEntryException("dbobject not define SystemGeneration key field : " + t.ToString());
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

        public static T GetAttribute<T>(MemberAdapter fi) where T : Attribute
        {
            T[] ts = (T[])fi.GetCustomAttributes(typeof(T), false);
            if (ts.Length == 1)
            {
                return ts[0];
            }
            return null;
        }

        public static Attribute GetAttribute(Type type, MemberAdapter fi)
        {
            Attribute[] ts = (Attribute[])fi.GetCustomAttributes(type, false);
            if (ts.Length == 1)
            {
                return ts[0];
            }
            return null;
        }

        public static string GetColumuName(MemberAdapter fi)
        {
            DbColumnAttribute fn = GetAttribute<DbColumnAttribute>(fi);
            return (fn == null) ? fi.Name : fn.Name;
        }

        internal static MemberHandler GetMemberHandler(MemberAdapter fi)
        {
            DbColumnAttribute fn = GetAttribute<DbColumnAttribute>(fi);
            string Name = (fn == null) ? fi.Name : fn.Name;

            MemberHandler fh = MemberHandler.NewObject(fi, Name);

            DbKeyAttribute dk = GetAttribute<DbKeyAttribute>(fi);
            if (dk != null)
            {
                fh.IsKey = true;
                if (dk.IsDbGenerate)
                {
                    fh.IsDbGenerate = true;
                }
                if (dk.UnsavedValue == null && dk.IsDbGenerate)
                {
                    if(fi.MemberType == typeof(long))
                    {
                        fh.UnsavedValue = 0L;
                    }
                    else if(fi.MemberType == typeof(int))
                    {
                        fh.UnsavedValue = 0;
                    }
                    else if(fi.MemberType == typeof(Guid))
                    {
                        fh.IsDbGenerate = false;
                        fh.UnsavedValue = Guid.Empty;
                    }
                    else
                    {
                        throw new DbEntryException("Unknown type of db key must set UnsavedValue");
                    }
                }
                else
                {
                    fh.UnsavedValue = dk.UnsavedValue;
                }
            }

            if (fi.MemberType.IsGenericType)
            {
                Type t = typeof(HasOne<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t)
                {
                    fh.IsHasOne = true;
                }
                Type t0 = typeof(HasMany<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t0)
                {
                    fh.IsHasMany = true;
                }
                Type t1 = typeof(BelongsTo<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t1)
                {
                    fh.IsBelongsTo = true;
                    if (fn == null)
                    {
                        Type ot = fi.MemberType.GetGenericArguments()[0];
                        string n = GetObjectFromClause(ot).GetMainTableName();
                        fh.Name = n + "_Id";
                    }
                }
                Type t2 = typeof(HasAndBelongsToMany<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t2)
                {
                    fh.IsHasAndBelongsToMany = true;
                    if (fn == null)
                    {
                        Type ot1 = fi.MemberType.GetGenericArguments()[0];
                        string n1 = GetObjectFromClause(ot1).GetMainTableName();
                        fh.Name = n1 + "_Id";
                    }
                }
                Type t3 = typeof(LazyLoadField<>);
                if (fi.MemberType.GetGenericTypeDefinition() == t3)
                {
                    fh.IsLazyLoad = true;
                }
            }

            if (GetAttribute<AllowNullAttribute>(fi) != null || NullableHelper.IsNullableType(fi.MemberType))
            {
                fh.AllowNull = true;
            }

            if (GetAttribute<SpecialNameAttribute>(fi) != null)
            {
                if (fi.Name == "CreatedOn")
                {
                    if (fi.MemberType == typeof(DateTime))
                    {
                        fh.IsCreatedOn = true;
                    }
                    else
                    {
                        throw new DbEntryException("CreatedOn must be datetime type.");
                    }
                }
                else if (fi.Name == "UpdatedOn")
                {
                    if (fi.MemberType == typeof(DateTime?))
                    {
                        fh.IsUpdatedOn = true;
                    }
                    else
                    {
                        throw new DbEntryException("UpdatedOn must be nullable datetime type.");
                    }
                }
                else
                {
                    throw new DbEntryException("Only CreatedOn and UpdatedOn are supported as special name.");
                }
            }

            MaxLengthAttribute ml = GetAttribute<MaxLengthAttribute>(fi);
            if (ml != null)
            {
                if (fi.MemberType.IsSubclassOf(typeof(ValueType)))
                {
                    throw new DbEntryException("ValueType couldn't set MaxLengthAttribute!");
                }
                fh.MaxLength = ml.Value;
            }

            if (fi.MemberType == typeof(string) || 
                (fh.IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string)))
            {
                fh.IsUnicode = true;
            }
            StringColumnAttribute sf = GetAttribute<StringColumnAttribute>(fi);
            if (sf != null)
            {
                if (!(fi.MemberType == typeof(string) || (fh.IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string))))
                {
                    throw new DbEntryException("StringFieldAttribute must set for String Type Field!");
                }
                fh.IsUnicode = sf.IsUnicode;
                fh.Regular = sf.Regular;
            }
            OrderByAttribute os = GetAttribute<OrderByAttribute>(fi);
            if (os != null)
            {
                fh.OrderByString = os.OrderBy;
            }
            return fh;
        }

        private static void ProcessMember(MemberAdapter m, List<MemberHandler> ret, List<MemberHandler> kfs)
        {
            if (!HasAtributes(m, typeof(ExcludeAttribute), typeof(HasOneAttribute), typeof(HasManyAttribute),
                typeof(HasAndBelongsToManyAttribute), typeof(BelongsToAttribute), typeof(LazyLoadAttribute)))
            {
                MemberHandler fh = GetMemberHandler(m);
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

        private static bool HasAtributes(MemberAdapter m, params Type[] atts)
        {
            foreach (Type att in atts)
            {
                if (GetAttribute(att, m) != null)
                {
                    return true;
                }
            }
            return false;
        }

        private static ObjectInfo m_GetObjectInfo(Type tt)
        {
            Type t = (tt.IsAbstract) ? DynamicObject.GetImplType(tt) : tt;
            if (ObjectInfos.Contains(t))
            {
                return (ObjectInfo)ObjectInfos[t];
            }
            else
            {
                ObjectInfo oi = m_GetSimpleObjectInfo(t);
                // binding QueryComposer
                oi.Composer = string.IsNullOrEmpty(oi.SoftDeleteColumnName) ?
                    new QueryComposer(oi) :
                    new SoftDeleteQueryComposer(oi, oi.SoftDeleteColumnName);
                // binding DbObjectHandler
                if (DataSetting.ObjectHandlerType == HandlerType.Emit
                    || (DataSetting.ObjectHandlerType == HandlerType.Both && t.IsPublic))
                {
                    oi.Handler = DynamicObject.CreateDbObjectHandler(t, oi);
                }
                else
                {
                    oi.Handler = new ReflectionDbObjectHandler(t, oi);
                }

                lock (ObjectInfos.SyncRoot)
                {
                    ObjectInfos[t] = oi;
                }
                return oi;
            }
        }

        internal static ObjectInfo GetObjectInfoOnly(Type tt)
        {
            Type t = (tt.IsAbstract) ? DynamicObject.GetImplType(tt) : tt;
            if (ObjectInfos.Contains(t))
            {
                return (ObjectInfo)ObjectInfos[t];
            }
            else
            {
                return m_GetSimpleObjectInfo(t);
            }
        }

        private static ObjectInfo m_GetSimpleObjectInfo(Type t)
        {
            List<MemberHandler> ret = new List<MemberHandler>();
            List<MemberHandler> kfs = new List<MemberHandler>();
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
                        throw new DbEntryException("Multi key did not allow SystemGeneration!");
                    }
                }
            }

            // fill simple and relation fields.
            List<MemberHandler> rlfs = new List<MemberHandler>();
            List<MemberHandler> sifs = new List<MemberHandler>();
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
            List<MemberHandler> fields = new List<MemberHandler>(sifs);
            fields.AddRange(rlfs);
            MemberHandler[] keys = kfs.ToArray();

            ObjectInfo oi = new ObjectInfo(t, GetObjectFromClause(t), keys, fields.ToArray(), DisableSqlLog(t));
            SetManyToManyMediFrom(oi, t, oi.From.GetMainTableName(), oi.Fields);

            oi.RelationFields = rlfs.ToArray();
            oi.SimpleFields = sifs.ToArray();

            SoftDeleteAttribute[] sdas = (SoftDeleteAttribute[])t.GetCustomAttributes(typeof(SoftDeleteAttribute), true);
            if (sdas != null && sdas.Length > 0)
            {
                oi.SoftDeleteColumnName = sdas[0].ColumnName;
            }

            return oi;
        }

        internal static MemberHandler GetKeyField(Type tt)
        {
            Type t = (tt.IsAbstract) ? DynamicObject.GetImplType(tt) : tt;
            if (ObjectInfos.Contains(t))
            {
                return ((ObjectInfo)ObjectInfos[t]).KeyFields[0];
            }
            else
            {
                foreach (PropertyInfo pi in t.GetProperties(ClassHelper.InstancePublic))
                {
                    if (!pi.PropertyType.IsGenericType)
                    {
                        MemberAdapter m = MemberAdapter.NewObject(pi);
                        if (HasAtributes(m, typeof(DbKeyAttribute)))
                        {
                            MemberHandler mh = GetMemberHandler(m);
                            return mh;
                        }
                    }
                }
                foreach (FieldInfo fi in t.GetFields(ClassHelper.InstanceFlag))
                {
                    if (!fi.IsPrivate)
                    {
                        if (!fi.FieldType.IsGenericType)
                        {
                            MemberAdapter m = MemberAdapter.NewObject(fi);
                            if (HasAtributes(m, typeof(DbKeyAttribute)))
                            {
                                MemberHandler mh = GetMemberHandler(m);
                                return mh;
                            }
                        }
                    }
                }
                return null;
            }
        }

        private static void SetManyToManyMediFrom(ObjectInfo oi, Type t, string MainTableName, MemberHandler[] Fields)
        {
            foreach (MemberHandler f in Fields)
            {
                if (f.IsHasAndBelongsToMany)
                {
                    Type ft = f.FieldType.GetGenericArguments()[0];
                    string SlaveTableName = GetObjectFromClause(ft).GetMainTableName();
                    string MediTableName = MainTableName.CompareTo(SlaveTableName) > 0 ?
                        SlaveTableName + "_" + MainTableName : MainTableName + "_" + SlaveTableName;
                    oi.ManyToManyMediTableName = MediTableName;
                    oi.ManyToManyMediColumeName1 = MainTableName + "_Id";
                    oi.ManyToManyMediColumeName2 = SlaveTableName + "_Id";
                    FromClause fc = new FromClause(
                        new JoinClause(MediTableName + "." + SlaveTableName + "_Id", SlaveTableName + ".Id",
                            CompareOpration.Equal, JoinMode.Inner));
                    oi.ManyToManyMediFrom = fc;
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

        private static FromClause GetObjectFromClause(Type DbObjectType)
        {
            DbTableAttribute[] dtas = (DbTableAttribute[])DbObjectType.GetCustomAttributes(typeof(DbTableAttribute), false);
            JoinOnAttribute[] joas = (JoinOnAttribute[])DbObjectType.GetCustomAttributes(typeof(JoinOnAttribute), false);
            if (dtas.Length != 0 && joas.Length != 0)
            {
                throw new ArgumentException(string.Format("class [{0}] defined DbTable and JoinOn. Only one allowed.", DbObjectType.Name));
            }
            if (dtas.Length == 0)
            {
                if (joas.Length == 0)
                {
                    return new FromClause(GetTableNameFromConfig(DbObjectType.Name));
                }
                JoinClause[] jcs = new JoinClause[joas.Length];
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

        private static string GetTableNameFromConfig(string DefinedName)
        {
            return ConfigHelper.DefaultSettings.GetValue("@" + DefinedName, DefinedName);
        }
    }
}
