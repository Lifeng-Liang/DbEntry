using System;
using System.Collections.Generic;
using System.Data;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.Core;
using Lephone.Core.Logging;

namespace Lephone.Data.Common
{
    public partial class ObjectInfo
    {
        public static object CreateObject(Type dbObjectType, IDataReader dr, bool useIndex)
        {
            if(dbObjectType.Name.StartsWith("<"))
            {
                return DynamicLinqObjectHandler.Factory.GetInstance(dbObjectType).CreateObject(dr, useIndex);
            }

            var oi = Factory.GetInstance(dbObjectType);
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
            ObjectInfo oi = Factory.GetInstance(dbObjectType);
            return oi.From;
        }

        public static Condition GetKeyWhereClause(object obj)
        {
            Type t = obj.GetType();
            ObjectInfo oi = Factory.GetInstance(t);
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
            ObjectInfo oi = Factory.GetInstance(t);
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
            var oi = Factory.GetSimpleInstance(tt);
            if (oi.KeyFields.Length > 0)
            {
                return oi.KeyFields[0];
            }
            return null;
        }

        #region shortcut functions

        public object NewObject()
        {
            return Handler.CreateInstance();
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
            var oi = Factory.GetInstance(obj.GetType());
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
                        ObjectInfo oi1 = Factory.GetInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler h1 = oi1.GetBelongsTo(oi.HandleType);
                        ho.Init(h1.Name);
                    }
                    else if (f.IsHasAndBelongsToMany)
                    {
                        ObjectInfo oi1 = Factory.GetInstance(f.FieldType.GetGenericArguments()[0]);
                        MemberHandler h1 = oi1.GetHasAndBelongsToMany(oi.HandleType);
                        ho.Init(h1.Name);
                    }
                }
            }
        }

        #endregion
    }
}
