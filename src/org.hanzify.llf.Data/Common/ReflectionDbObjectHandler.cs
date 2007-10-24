
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection;
using Lephone.Data.Builder;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Common
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
                    //SetValue(f, o, dr[i++]);
                    f.SetValue(o, dr[i++]);
                }
            }
            else
            {
                foreach (MemberHandler f in oi.SimpleFields)
                {
                    //SetValue(f, o, dr[f.Name]);
                    f.SetValue(o, dr[f.Name]);
                }
            }
        }

        private void SetValue(MemberHandler f, object o, object v)
        {
            if (v.GetType() == typeof(decimal))
            {
                if (f.FieldType == typeof(decimal))
                {
                    f.SetValue(o, v);
                }
                else
                {
                    if (f.FieldType.IsEnum)
                    {
                        f.SetValue(o, Convert.ChangeType(v, typeof(int)));
                    }
                    else
                    {
                        if (f.FieldType.IsGenericType)
                        {
                            f.SetValue(o, Convert.ChangeType(v, f.FieldType.GetGenericArguments()[0]));
                        }
                        else
                        {
                            f.SetValue(o, Convert.ChangeType(v, f.FieldType));
                        }
                    }
                }
            }
            else
            {
                f.SetValue(o, v);
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
                    ObjectInfo oi1 = ObjectInfo.GetInstance(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetBelongsTo(oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(driver, h1.Name);
                    }
                    else
                    {
                        // TODO: should throw exception or not ?
                        throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
                        // ho.Init(driver, "__");
                    }
                }
                else if (f.IsHasAndBelongsToMany)
                {
                    ObjectInfo oi1 = ObjectInfo.GetInstance(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetHasAndBelongsToMany(oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(driver, h1.Name);
                    }
                    else
                    {
                        // TODO: should throw exception or not ?
                        throw new DataException("HasOne or HasMany and BelongsTo must be paired.");
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
                throw new DataException("The class must and just have one key");
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
            else if (fi.IsCreatedOn || fi.IsUpdatedOn)
            {
                isv.Values.Add(new KeyValue(fi.Name, DbNow.Value));
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
                if (!fi.IsDbGenerate && !fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany &&!fi.IsUpdatedOn)
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
                    if (fi.IsUpdatedOn || (!fi.IsCreatedOn && to.m_UpdateColumns.ContainsKey(fi.Name)))
                    {
                        AddKeyValue(isv, fi, obj);
                    }
                }
            }
            else
            {
                foreach (MemberHandler fi in oi.Fields)
                {
                    if (fi.IsUpdatedOn || (!fi.IsCreatedOn && !fi.IsDbGenerate && !fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany))
                    {
                        AddKeyValue(isv, fi, obj);
                    }
                }
            }
        }
    }
}
