
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.Definition;
using org.hanzify.llf.Data.SqlEntry;

namespace org.hanzify.llf.Data.Common
{
    internal class ReflectionDbObjectHandler : IDbObjectHandler
    {
        private static readonly object[] os = new object[] { };

        private ConstructorInfo Creator;
        private ObjectInfo oi;

        public ReflectionDbObjectHandler(Type srcType, ObjectInfo oi)
        {
            ConstructorInfo ci = srcType.GetConstructor(new Type[] { });
            Creator = ci;
            this.oi = oi;
        }

        public object CreateInstance()
        {
            return Creator.Invoke(os);
        }

        public void LoadSimpleValues(object o, bool UseIndex, IDataReader dr)
        {
            if (UseIndex)
            {
                int i = 0;
                foreach (MemberHandler f in oi.SimpleFields)
                {
                    f.SetValue(o, dr[i++]);
                }
            }
            else
            {
                foreach (MemberHandler f in oi.SimpleFields)
                {
                    f.SetValue(o, dr[f.Name]);
                }
            }
        }

        public void LoadRelationValues(DbContext driver, object o, bool UseIndex, IDataReader dr)
        {
            int n = oi.SimpleFields.Length;
            foreach (MemberHandler f in oi.RelationFields)
            {
                ILazyLoading ho = (ILazyLoading)f.GetValue(o);
                if (f.IsLazyLoad)
                {
                    ho.Init(driver, f.Name);
                }
                else if (f.IsHasOne || f.IsHasMany)
                {
                    ObjectInfo oi1 = DbObjectHelper.GetObjectInfo(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetBelongsTo(oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(driver, h1.Name);
                    }
                    else
                    {
                        // TODO: should throw exception or not ?
                        throw new DbEntryException("HasOne or HasMany and BelongsTo must be paired.");
                        // ho.Init(driver, "__");
                    }
                }
                else if (f.IsHasAndBelongsToMany)
                {
                    ObjectInfo oi1 = DbObjectHelper.GetObjectInfo(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetHasAndBelongsToMany(oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(driver, h1.Name);
                    }
                    else
                    {
                        // TODO: should throw exception or not ?
                        throw new DbEntryException("HasOne or HasMany and BelongsTo must be paired.");
                        // ho.Init(driver, "__");
                    }
                }
                else if (f.IsBelongsTo) // TODO: IsHasAndBelongsToMany
                {
                    IBelongsTo hbo = (IBelongsTo)ho;
                    if (UseIndex)
                    {
                        hbo.ForeignKey = dr[n++];
                    }
                    else
                    {
                        hbo.ForeignKey = dr[f.Name];
                    }
                }
            }
        }

        public Dictionary<string, object> GetKeyValues(object o)
        {
            Dictionary<string, object> dic = new Dictionary<string,object>();
            foreach (MemberHandler mh in oi.KeyFields)
            {
                dic.Add(mh.Name, mh.GetValue(o));
            }
            return dic;
        }

        public object GetKeyValue(object o)
        {
            if (oi.KeyFields.Length != 1)
            {
                throw new DbEntryException("The class must and just have one key");
            }
            return oi.KeyFields[0].GetValue(o);
        }

        public void SetValuesForSelect(ISqlKeys isv)
        {
            foreach (MemberHandler fi in oi.Fields)
            {
                if (!fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany && !fi.IsLazyLoad)
                {
                    isv.Keys.Add(fi.Name);
                }
            }
        }

        private static void AddKeyValue(ISqlValues isv, MemberHandler fi, object obj)
        {
            object value = fi.GetValue(obj);
            if (fi.IsBelongsTo)
            {
                IBelongsTo ll = value as IBelongsTo;
                Type fkt = (ll.ForeignKey != null) ? ll.ForeignKey.GetType() : typeof(int);
                KeyValue kv = new KeyValue(fi.Name, ll.ForeignKey, fkt);
                isv.Values.Add(kv);
            }
            else if (fi.IsLazyLoad)
            {
                ILazyLoading ll = (ILazyLoading)value;
                ll.IsLoaded = true;
                object ov = ll.Read();
                Type t = fi.FieldType.GetGenericArguments()[0];
                KeyValue kv = new KeyValue(fi.Name, ov, t);
                isv.Values.Add(kv);
            }
            else
            {
                KeyValue kv = new KeyValue(fi.Name, value, fi.FieldType);
                isv.Values.Add(kv);
            }
        }

        public void SetValuesForInsert(ISqlValues isv, object obj)
        {
            Type t = obj.GetType();
            foreach (MemberHandler fi in oi.Fields)
            {
                if (!fi.IsDbGenerate && !fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany)
                {
                    AddKeyValue(isv, fi, obj);
                }
            }
        }

        public void SetValuesForUpdate(ISqlValues isv, object obj)
        {
            Type t = obj.GetType();
            DbObjectSmartUpdate to = obj as DbObjectSmartUpdate;
            if (to != null && to.m_UpdateColumns != null)
            {
                foreach (MemberHandler fi in oi.Fields)
                {
                    //if (to.m_UpdateColumns.ContainsKey(fi.IsBelongsTo ? "$" : fi.Name))
                    if (to.m_UpdateColumns.ContainsKey(fi.Name))
                    {
                        AddKeyValue(isv, fi, obj);
                    }
                }
            }
            else
            {
                foreach (MemberHandler fi in oi.Fields)
                {
                    if (!fi.IsDbGenerate && !fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany)
                    {
                        AddKeyValue(isv, fi, obj);
                    }
                }
            }
        }
    }
}
