
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
        internal static void InitObjectInfoBySimpleMode(Type t, ObjectInfo oi)
        {
            List<Type> lt = new List<Type>(t.GetInterfaces());
            if (!lt.Contains(typeof(IDbObject)))
            {
                throw new DataException("The data object must implements IDbObject!");
            }

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
                        throw new DataException("Multiple key do not allow SystemGeneration!");
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

            oi.Init(t, GetObjectFromClause(t), keys, fields.ToArray(), DisableSqlLog(t));
            SetManyToManyMediFrom(oi, t, oi.From.GetMainTableName(), oi.Fields);

            oi._RelationFields = rlfs.ToArray();
            oi._SimpleFields = sifs.ToArray();

            SoftDeleteAttribute sd = ClassHelper.GetAttribute<SoftDeleteAttribute>(t, true);
            if (sd != null)
            {
                oi._SoftDeleteColumnName = sd.ColumnName;
            }
            DeleteToAttribute dta = ClassHelper.GetAttribute<DeleteToAttribute>(t, true);
            if (dta != null)
            {
                oi._DeleteToTableName = dta.TableName;
            }

            GetIndexes(oi);
        }

        public static object CreateObject(DbContext context, Type DbObjectType, IDataReader dr, bool UseIndex)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
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

        public static FromClause GetFromClause(Type DbObjectType)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(DbObjectType);
            return oi.From;
        }

        public static WhereCondition GetKeyWhereClause(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            if (oi.KeyFields == null)
            {
                throw new DataException("dbobject not define key field : " + t.ToString());
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
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            if (!oi.HasSystemKey)
            {
                throw new DataException("dbobject not define SystemGeneration key field : " + t.ToString());
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
            DbColumnAttribute fn = fi.GetAttribute<DbColumnAttribute>(false);
            return (fn == null) ? fi.Name : fn.Name;
        }

        internal static MemberHandler GetMemberHandler(MemberAdapter fi)
        {
            DbColumnAttribute fn = fi.GetAttribute<DbColumnAttribute>(false);
            string Name = (fn == null) ? fi.Name : fn.Name;

            MemberHandler fh = MemberHandler.NewObject(fi, Name);

            DbKeyAttribute dk = fi.GetAttribute<DbKeyAttribute>(false);
            if (dk != null)
            {
                fh.IsKey = true;
                if (dk.IsDbGenerate)
                {
                    fh.IsDbGenerate = true;
                }
                if (dk.UnsavedValue == null && dk.IsDbGenerate)
                {
                    fh.UnsavedValue = CommonHelper.GetEmptyValue(fi.MemberType, false, "Unknown type of db key must set UnsavedValue");
                    if(fi.MemberType == typeof(Guid))
                    {
                        fh.IsDbGenerate = false;
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

            if (fi.GetAttribute<AllowNullAttribute>(false) != null || NullableHelper.IsNullableType(fi.MemberType))
            {
                fh.AllowNull = true;
            }

            if (fi.GetAttribute<SpecialNameAttribute>(false) != null)
            {
                if (fi.Name == "CreatedOn")
                {
                    if (fi.MemberType == typeof(DateTime))
                    {
                        fh.IsCreatedOn = true;
                    }
                    else
                    {
                        throw new DataException("CreatedOn must be datetime type.");
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
                        throw new DataException("UpdatedOn must be nullable datetime type.");
                    }
                }
                else if (fi.Name == "LockVersion")
                {
                    if(fi.MemberType == typeof(int))
                    {
                        fh.IsLockVersion = true;
                    }
                    else
                    {
                        throw new DataException("LockVersion must be int type.");
                    }
                }
                else
                {
                    throw new DataException("Only CreatedOn and UpdatedOn are supported as special name.");
                }
            }

            LengthAttribute ml = fi.GetAttribute<LengthAttribute>(false);
            if (ml != null)
            {
                if (fi.MemberType.IsSubclassOf(typeof(ValueType)))
                {
                    throw new DataException("ValueType couldn't set MaxLengthAttribute!");
                }
                fh.MinLength = ml.Min;
                fh.MaxLength = ml.Max;
            }

            if (fi.MemberType == typeof(string) || 
                (fh.IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string)))
            {
                fh.IsUnicode = true;
            }
            StringColumnAttribute sf = fi.GetAttribute<StringColumnAttribute>(false);
            if (sf != null)
            {
                if (!(fi.MemberType == typeof(string) || (fh.IsLazyLoad && fi.MemberType.GetGenericArguments()[0] == typeof(string))))
                {
                    throw new DataException("StringFieldAttribute must set for String Type Field!");
                }
                fh.IsUnicode = sf.IsUnicode;
                fh.Regular = sf.Regular;
            }
            OrderByAttribute os = fi.GetAttribute<OrderByAttribute>(false);
            if (os != null)
            {
                fh.OrderByString = os.OrderBy;
            }
            return fh;
        }

        private static void ProcessMember(MemberAdapter m, List<MemberHandler> ret, List<MemberHandler> kfs)
        {
            if (!(
                m.HasAttribute<ExcludeAttribute>(false) ||
                m.HasAttribute<HasOneAttribute>(false) ||
                m.HasAttribute<HasManyAttribute>(false) ||
                m.HasAttribute<HasAndBelongsToManyAttribute>(false) ||
                m.HasAttribute<BelongsToAttribute>(false) ||
                m.HasAttribute<LazyLoadAttribute>(false)))
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

        private static void GetIndexes(ObjectInfo oi)
        {
            foreach (MemberHandler fh in oi.Fields)
            {
                IndexAttribute[] ias = (IndexAttribute[])fh.MemberInfo.GetAttributes<IndexAttribute>(false);
                CheckIndexAttributes(ias);
                foreach (IndexAttribute ia in ias)
                {
                    ASC a = ia.ASC ? (ASC)fh.Name : (DESC)fh.Name;
                    string key = (ia.IndexName == null) ? a.Key : ia.IndexName;
                    if (!oi.Indexes.ContainsKey(key))
                    {
                        oi.Indexes.Add(key, new List<ASC>());
                    }
                    oi.Indexes[key].Add(a);
                    if (ia.UNIQUE)
                    {
                        if (!oi.UniqueIndexes.ContainsKey(key))
                        {
                            oi.UniqueIndexes.Add(key, new List<MemberHandler>());
                        }
                        oi.UniqueIndexes[key].Add(fh);
                    }
                }
            }
        }

        internal static MemberHandler GetKeyField(Type tt)
        {
            ObjectInfo oi = ObjectInfo.GetSimpleInstance(tt);
            if (oi.KeyFields.Length > 0)
            {
                return oi.KeyFields[0];
            }
            return null;
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

                    FromClause fc = new FromClause(
                        new JoinClause(MediTableName + "." + SlaveTableName + "_Id", SlaveTableName + ".Id",
                            CompareOpration.Equal, JoinMode.Inner));
                    Type t2 = f.FieldType.GetGenericArguments()[0];
                    oi.ManyToManys[t2] 
                        = new ManyToManyMediTable(t2, fc, MediTableName, MainTableName + "_Id", SlaveTableName + "_Id");
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
