using System;
using System.Collections.Generic;
using System.Data;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.Util;
using Lephone.Util.Logging;
using Lephone.Util.Setting;
using Lephone.Util.Text;

namespace Lephone.Data.Common
{
    public partial class ObjectInfo
    {
        public static object CreateObject(Type dbObjectType, IDataReader dr, bool useIndex)
        {
            ObjectInfo oi = GetInstance(dbObjectType);
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
                    var bt = (ILazyLoading) mh.GetValue(obj);
                    bt.Init(mh.Name);
                }
            }
            oi.Handler.LoadSimpleValues(obj, useIndex, dr);
            oi.Handler.LoadRelationValues(obj, useIndex, dr);
            if (sudi != null)
            {
                sudi.m_InternalInit = false;
            }
            return obj;
        }

        public static FromClause GetFromClause(Type dbObjectType)
        {
            ObjectInfo oi = GetInstance(dbObjectType);
            return oi.From;
        }

        public static Condition GetKeyWhereClause(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = GetInstance(t);
            if (oi.KeyFields == null)
            {
                throw new DataException("dbobject not define key field : " + t);
            }
            Condition ret = null;
            Dictionary<string, object> dictionary = oi.Handler.GetKeyValues(obj);
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
            if (fh.FieldType == typeof (long))
            {
                sKey = Convert.ToInt64(key);
            }
            else
            {
                sKey = fh.FieldType == typeof(int) ? Convert.ToInt32(key) : key;
            } 
            fh.SetValue(obj, sKey);
        }

        public static string GetColumuName(MemberAdapter fi)
        {
            var fn = fi.GetAttribute<DbColumnAttribute>(false);
            return (fn == null) ? fi.Name : fn.Name;
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

        internal static FromClause GetObjectFromClause(Type dbObjectType)
        {
            var userType = dbObjectType.Name.StartsWith("$") ? dbObjectType.BaseType : dbObjectType;
            var dtas = (DbTableAttribute[]) userType.GetCustomAttributes(typeof (DbTableAttribute), false);
            var joas = (JoinOnAttribute[]) userType.GetCustomAttributes(typeof (JoinOnAttribute), false);
            if (dtas.Length != 0 && joas.Length != 0)
            {
                throw new ArgumentException(string.Format("class [{0}] defined DbTable and JoinOn. Only one allowed.",
                                                          userType.Name));
            }
            if (dtas.Length == 0)
            {
                if (joas.Length == 0)
                {
                    string defaultName = NameMapper.Instance.MapName(userType.Name);
                    return new FromClause(GetTableNameFromConfig(defaultName));
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
                                                                  userType.Name));
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

        private static string GetTableNameFromConfig(string definedName)
        {
            return ConfigHelper.DefaultSettings.GetValue("@" + definedName, definedName);
        }

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

        public static object CloneObject(object obj)
        {
            if (obj == null) { return null; }
            ObjectInfo oi = GetInstance(obj.GetType());
            object o = oi.NewObject();
            var os = o as DbObjectSmartUpdate;
            if (os != null)
            {
                os.m_InternalInit = true;
                InnerCloneObject(obj, oi, o);
                os.m_InternalInit = false;
            }
            else
            {
                InnerCloneObject(obj, oi, o);
            }
            return o;
        }

        private static void InnerCloneObject(object obj, ObjectInfo oi, object o)
        {
            foreach (var mh in oi.RelationFields)
            {
                if (mh.IsBelongsTo || mh.IsHasAndBelongsToMany)
                {
                    var bt = (ILazyLoading)mh.GetValue(o);
                    bt.Init(mh.Name);
                }
            }
            foreach (var m in oi.SimpleFields)
            {
                object v = m.GetValue(obj);
                m.SetValue(o, v);
            }
            foreach (var f in oi.RelationFields)
            {
                if (f.IsBelongsTo)
                {
                    var os = (IBelongsTo)f.GetValue(obj);
                    var od = (IBelongsTo)f.GetValue(o);
                    od.ForeignKey = os.ForeignKey;
                }
                else
                {
                    var ho = (ILazyLoading)f.GetValue(o);
                    if (f.IsLazyLoad)
                    {
                        ho.Init(f.Name);
                    }
                    else if (f.IsHasOne || f.IsHasMany)
                    {
                        ObjectInfo oi1 = GetInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler h1 = oi1.GetBelongsTo(oi.HandleType);
                        ho.Init(h1.Name);
                    }
                    else if (f.IsHasAndBelongsToMany)
                    {
                        ObjectInfo oi1 = GetInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler h1 = oi1.GetHasAndBelongsToMany(oi.HandleType);
                        ho.Init(h1.Name);
                    }
                }
            }
        }

        #endregion
    }
}
