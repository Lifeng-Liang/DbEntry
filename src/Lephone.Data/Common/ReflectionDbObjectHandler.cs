using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Lephone.Data.Builder;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Common
{
    internal class ReflectionDbObjectHandler : IDbObjectHandler
    {
        private static readonly object[] os = new object[] { };

        private readonly ConstructorInfo _creator;
        private readonly ObjectInfo _oi;

        public ReflectionDbObjectHandler(Type srcType, ObjectInfo oi)
        {
            ConstructorInfo ci = srcType.GetConstructor(new Type[] { });
            _creator = ci;
            this._oi = oi;
        }

        public object CreateInstance()
        {
            return _creator.Invoke(os);
        }

        public void LoadSimpleValues(object o, bool useIndex, IDataReader dr)
        {
            if (useIndex)
            {
                int i = 0;
                foreach (MemberHandler f in _oi.SimpleFields)
                {
                    //SetValue(f, o, dr[i++]);
                    f.SetValue(o, dr[i++]);
                }
            }
            else
            {
                foreach (MemberHandler f in _oi.SimpleFields)
                {
                    //SetValue(f, o, dr[f.Name]);
                    f.SetValue(o, dr[f.Name]);
                }
            }
        }

        public void LoadRelationValues(object o, bool useIndex, IDataReader dr)
        {
            int n = _oi.SimpleFields.Length;
            foreach (MemberHandler f in _oi.RelationFields)
            {
                var ho = (ILazyLoading)f.GetValue(o);
                if (f.IsLazyLoad)
                {
                    ho.Init(f.Name);
                }
                else if (f.IsHasOne || f.IsHasMany)
                {
                    ObjectInfo oi1 = ObjectInfo.GetInstance(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetBelongsTo(_oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(h1.Name);
                    }
                    else
                    {
                        throw new DataException("HasOne/HasMany and BelongsTo must be paired.");
                    }
                }
                else if (f.IsHasAndBelongsToMany)
                {
                    ObjectInfo oi1 = ObjectInfo.GetInstance(f.FieldType.GetGenericArguments()[0]);
                    MemberHandler h1 = oi1.GetHasAndBelongsToMany(_oi.HandleType);
                    if (h1 != null)
                    {
                        ho.Init(h1.Name);
                    }
                    else
                    {
                        throw new DataException("HasOne/HasMany and BelongsTo must be paired.");
                    }
                }
                else if (f.IsBelongsTo) // TODO: IsHasAndBelongsToMany
                {
                    var hbo = (IBelongsTo)ho;
                    hbo.ForeignKey = useIndex ? dr[n++] : dr[f.Name];
                }
            }
        }

        public Dictionary<string, object> GetKeyValues(object o)
        {
            var dic = new Dictionary<string,object>();
            foreach (MemberHandler mh in _oi.KeyFields)
            {
                dic.Add(mh.Name, mh.GetValue(o));
            }
            return dic;
        }

        public object GetKeyValue(object o)
        {
            if (_oi.KeyFields.Length != 1)
            {
                throw new DataException("The class must and just have one key");
            }
            return _oi.KeyFields[0].GetValue(o);
        }

        public void SetValuesForSelect(ISqlKeys isv)
        {
            foreach (MemberHandler fi in _oi.Fields)
            {
                if (!fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany && !fi.IsLazyLoad)
                {
                    string value = null;
                    if(fi.Name != fi.MemberInfo.Name)
                    {
                        value = fi.MemberInfo.Name;
                    }
                    isv.Keys.Add(new KeyValuePair<string, string>(fi.Name, value));
                }
            }
        }

        private static void AddKeyValue(ISqlValues isv, MemberHandler fi, object obj)
        {
            object value = fi.GetValue(obj);
            if (fi.IsBelongsTo)
            {
                var ll = (IBelongsTo)value;
                Type fkt = (ll.ForeignKey != null) ? ll.ForeignKey.GetType() : typeof(int);
                var kv = new KeyValue(fi.Name, ll.ForeignKey, fkt);
                isv.Values.Add(kv);
            }
            else if (fi.IsLazyLoad)
            {
                var ll = (ILazyLoading)value;
                //ll.IsLoaded = true;
                object ov = ll.Read();
                Type t = fi.FieldType.GetGenericArguments()[0];
                var kv = new KeyValue(fi.Name, ov, t);
                isv.Values.Add(kv);
            }
            else if (fi.IsAutoSavedValue)
            {
                isv.Values.Add(new KeyValue(fi.Name, fi.IsCount || fi.IsLockVersion ? AutoValue.Count : AutoValue.DbNow));
            }
            else
            {
                var kv = new KeyValue(fi.Name, value, fi.FieldType);
                isv.Values.Add(kv);
            }
        }

        public void SetValuesForInsert(ISqlValues isv, object obj)
        {
            foreach (MemberHandler fi in _oi.Fields)
            {
                if (!fi.IsDbGenerate && !fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany && !fi.IsUpdatedOn)
                {
                    AddKeyValue(isv, fi, obj);
                }
            }
        }

        public void SetValuesForUpdate(ISqlValues isv, object obj)
        {
            var to = obj as DbObjectSmartUpdate;
            if (to != null && to.m_UpdateColumns != null)
            {
                foreach (MemberHandler fi in _oi.Fields)
                {
                    if(!fi.IsKey)
                    {
                        if (fi.IsUpdatedOn || fi.IsSavedOn || fi.IsCount || (!fi.IsLockVersion && !fi.IsCreatedOn && to.m_UpdateColumns.ContainsKey(fi.Name)))
                        {
                            AddKeyValue(isv, fi, obj);
                        }
                    }
                }
            }
            else
            {
                foreach (MemberHandler fi in _oi.Fields)
                {
                    if(!fi.IsKey)
                    {
                        if (fi.IsUpdatedOn || fi.IsSavedOn || fi.IsCount || (!fi.IsLockVersion && !fi.IsCreatedOn && !fi.IsDbGenerate && !fi.IsHasOne && !fi.IsHasMany && !fi.IsHasAndBelongsToMany))
                        {
                            AddKeyValue(isv, fi, obj);
                        }
                    }
                }
            }
        }
    }
}
