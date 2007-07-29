
#region usings

using System;
using System.Reflection;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using org.hanzify.llf.util;
using org.hanzify.llf.util.Setting;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.Builder.Clause;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.Data.Common
{
    internal static class DbObjectHelper
    {
        private static HybridDictionary ObjectInfos = new HybridDictionary();

        public static object CreateObject(DbDriver driver, Type DbObjectType, IDataReader dr, bool UseIndex)
        {
            if (UseIndex)
            {
                int i = 0;
                return InnerCreateObject(driver, DbObjectType, delegate(object o)
                {
                    return dr[i++];
                });
            }
            else
            {
                return InnerCreateObject(driver, DbObjectType, delegate(object o)
                {
                    return dr[((MemberHandler)o).Name];
                });
            }
        }

        private static object InnerCreateObject(DbDriver driver, Type DbObjectType,
            CallbackHandler<object,object> callback)
        {
            ObjectInfo ii = m_GetObjectInfo(DbObjectType);
            object di = ii.NewObject();
            DbObjectSmartUpdate sudi = di as DbObjectSmartUpdate;
            if (sudi != null)
            {
                sudi.m_InternalInit = true;
            }
            if (ii.BelongsToField != null)
            {
                ILazyLoading bt = (ILazyLoading)ii.BelongsToField.GetValue(di);
                bt.Init(driver, ii.BelongsToField.Name);
            }
            foreach (MemberHandler f in ii.Fields)
            {
                if (f.IsHasOne || f.IsHasMany || f.IsHasManyAndBelongsTo)
                {
                    ILazyLoading ho = (ILazyLoading)f.GetValue(di);
                    ObjectInfo oi = m_GetObjectInfo(f.FieldType.GetGenericArguments()[0]);
                    if (oi.BelongsToField != null)
                    {
                        ho.Init(driver, oi.BelongsToField.Name);
                    }
                    else
                    {
                        // TODO: should throw exception or not ?
                        throw new DbEntryException("HasOne or HasMany and BelongsTo must be paired.");
                        // ho.Init(driver, "__");
                    }
                }
                else if (f.IsBelongsTo) // TODO: IsHasManyAndBelongsTo
                {
                    IBelongsTo ho = (IBelongsTo)f.GetValue(di);
                    ho.ForeignKey = callback(f);
                }
                else
                {
                    f.SetValue(di, callback(f));
                }
            }
            if (sudi != null)
            {
                sudi.m_InternalInit = false;
            }
            return di;
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
            ObjectInfo ii = m_GetObjectInfo(t);
            Dictionary<string, List<ASC>> dict = new Dictionary<string,List<ASC>>();
            Dictionary<string, bool> UniIndex = new Dictionary<string, bool>();
            foreach( MemberHandler fh in ii.Fields)
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
            ObjectInfo ii = m_GetObjectInfo(DbObjectType);
            return ii.From;
        }

        public static void SetValues(ISqlValues isv, object obj, bool IncludeKey, bool SmartUpdate)
        {
            Type t = obj.GetType();
            ObjectInfo ii = m_GetObjectInfo(t);
            DbObjectSmartUpdate to = obj as DbObjectSmartUpdate;
            if (SmartUpdate && to != null && to.m_UpdateColumns != null)
            {
                foreach (MemberHandler fi in ii.Fields)
                {
                    if (to.m_UpdateColumns.ContainsKey(fi.Name))
                    {
                        AddKeyValue(isv, fi, obj);
                    }
                }
            }
            else
            {
                foreach (MemberHandler fi in ii.Fields)
                {
                    if ((!fi.IsSystemGeneration || IncludeKey) && !fi.IsHasOne && !fi.IsHasMany && !fi.IsHasManyAndBelongsTo)
                    {
                        AddKeyValue(isv, fi, obj);
                    }
                }
            }
        }

        private static void AddKeyValue(ISqlValues isv, MemberHandler fi, object obj)
        {
            if (fi.IsBelongsTo)
            {
                IBelongsTo ll = (IBelongsTo)fi.GetValue(obj);
                Type fkt = (ll.ForeignKey != null) ? ll.ForeignKey.GetType() : typeof(int);
                KeyValue kv = new KeyValue(fi.Name, ll.ForeignKey, fkt);
                isv.Values.Add(kv);
            }
            else
            {
                KeyValue kv = new KeyValue(fi.Name, fi.GetValue(obj), fi.FieldType);
                isv.Values.Add(kv);
            }
        }

        public static WhereCondition GetKeyWhereClause(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo ii = m_GetObjectInfo(t);
            if (ii.KeyFields == null)
            {
                throw new DbEntryException("dbobject not define key field : " + t.ToString());
            }
            WhereCondition ret = null;
            foreach (MemberHandler fh in ii.KeyFields)
            {
                ret &= (CK.K[fh.Name] == fh.GetValue(obj));
            }
            return ret;
        }

        public static void SetKey(object obj, object key)
        {
            Type t = obj.GetType();
            ObjectInfo ii = m_GetObjectInfo(t);
            if (!ii.HasSystemKey)
            {
                throw new DbEntryException("dbobject not define SystemGeneration key field : " + t.ToString());
            }
            MemberHandler fh = ii.KeyFields[0];
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

        private static MemberHandler GetMemberHandler(MemberAdapter fi)
        {
            DbColumnAttribute fn = GetAttribute<DbColumnAttribute>(fi);
            string Name = (fn == null) ? fi.Name : fn.Name;

            MemberHandler fh = MemberHandler.NewObject(fi, Name);

            DbKeyAttribute dk = GetAttribute<DbKeyAttribute>(fi);
            if (dk != null)
            {
                fh.IsKey = true;
                if (dk.IsSystemGeneration)
                {
                    fh.IsSystemGeneration = true;
                }
                if (dk.UnsavedValue != null && (!dk.IsSystemGeneration))
                {
                    throw new DbEntryException("Not SystemGeneration Key can not have a UnsavedValue!");
                }
                fh.UnsavedValue = dk.UnsavedValue;
            }

            if (fi.MemberType.IsGenericType)
            {
                Type t = typeof(HasOne<object>).GetGenericTypeDefinition();
                if (fi.MemberType.GetGenericTypeDefinition() == t)
                {
                    fh.IsHasOne = true;
                }
                Type t0 = typeof(HasMany<object>).GetGenericTypeDefinition();
                if (fi.MemberType.GetGenericTypeDefinition() == t0)
                {
                    fh.IsHasMany = true;
                }
                Type t1 = typeof(BelongsTo<object>).GetGenericTypeDefinition();
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
                Type t2 = typeof(HasManyAndBelongsTo<object>).GetGenericTypeDefinition();
                if (fi.MemberType.GetGenericTypeDefinition() == t2)
                {
                    fh.IsHasManyAndBelongsTo = true;
                    if (fn == null)
                    {
                        Type ot1 = fi.MemberType.GetGenericArguments()[0];
                        string n1 = GetObjectFromClause(ot1).GetMainTableName();
                        fh.Name = n1 + "_Id";
                    }
                }
                if (fh.IsBelongsTo || fh.IsHasMany || fh.IsHasOne || fh.IsHasManyAndBelongsTo)
                {
                    fh.IsLazyLoad = true;
                }
            }

            if (GetAttribute<AllowNullAttribute>(fi) != null || NullableHelper.IsNullableType(fi.MemberType))
            {
                fh.AllowNull = true;
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

            if (fi.MemberType == typeof(string))
            {
                fh.IsUnicode = true;
            }
            StringColumnAttribute sf = GetAttribute<StringColumnAttribute>(fi);
            if (sf != null)
            {
                if (fi.MemberType != typeof(string))
                {
                    throw new DbEntryException("StringFieldAttribute must set for String Type Field!");
                }
                fh.IsUnicode = sf.IsUnicode;
                fh.Regular = sf.Regular;
            }
            OrderByAttribute os = GetAttribute<OrderByAttribute>(fi);
            if (os != null)
            {
                if (fh.IsHasMany || fh.IsHasOne || fh.IsHasManyAndBelongsTo)
                {
                    fh.OrderByString = os.OrderBy;
                }
                else
                {
                    throw new DbEntryException("Only HasMany HasOne HasManyAndBelongsTo allows OrderBy attribute!");
                }
            }
            return fh;
        }

        private static void ProcessMember(MemberAdapter m, ArrayList ret, ArrayList kfs)
        {
            if (!HasAtributes(m, typeof(ExcludeAttribute), typeof(HasOneAttribute),
                typeof(HasManyAttribute), typeof(HasManyAndBelongsToAttribute), typeof(BelongsToAttribute)))
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
                ArrayList ret = new ArrayList();
                ArrayList kfs = new ArrayList();
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
                        if (k.IsSystemGeneration)
                        {
                            throw new DbEntryException("Multi key did not allow SystemGeneration!");
                        }
                    }
                }
                MemberHandler[] fhs = (MemberHandler[])ret.ToArray(typeof(MemberHandler));
                MemberHandler[] keys = (MemberHandler[])kfs.ToArray(typeof(MemberHandler));
                ObjectInfo ii = new ObjectInfo(GetObjectFromClause(t), keys, fhs, DisableSqlLog(t));
                SetManyToManyMediFrom(ii, t, ii.From.GetMainTableName(), fhs);
                ii.Constructor = t.GetConstructor(new Type[] { });
                lock (ObjectInfos.SyncRoot)
                {
                    ObjectInfos[t] = ii;
                }
                return ii;
            }
        }

        private static void SetManyToManyMediFrom(ObjectInfo oi, Type t, string MainTableName, MemberHandler[] Fields)
        {
            foreach (MemberHandler f in Fields)
            {
                if (f.IsHasManyAndBelongsTo)
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
