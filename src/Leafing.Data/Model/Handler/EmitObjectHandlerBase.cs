using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Definition;
using Leafing.Data.Model.Member;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Model.Handler
{
    public abstract class EmitObjectHandlerBase : IDbObjectHandler
    {
        protected KeyValue[] kvc;

        protected void AddKeyValue(KeyValueCollection values, DbObjectSmartUpdate o, string key, int n, object v)
        {
            if ((o.m_UpdateColumns == null) || o.m_UpdateColumns.ContainsKey(key))
            {
                values.Add(this.NewKeyValue(n, v));
            }
        }

        public abstract object CreateInstance();

        public object GetKeyValue(object o)
        {
            object keyValueDirect = this.GetKeyValueDirect(o);
            if (keyValueDirect == null)
            {
                throw new ModelException(o.GetType(), "The class must and just have one key.");
            }
            return keyValueDirect;
        }

        private static KeyValue GetKeyValue(ObjectInfo oi, MemberHandler fi)
        {
            if (fi.Is.BelongsTo)
            {
                Type type = fi.MemberType.GetGenericArguments()[0];
                MemberHandler handler = (oi.HandleType == type) ? oi.KeyMembers[0] : ModelContext.GetInstance(type).Info.KeyMembers[0];
                return new KeyValue(fi.Name, null, handler.MemberType);
            }
            if (fi.Is.LazyLoad)
            {
                return new KeyValue(fi.Name, null, fi.MemberType.GetGenericArguments()[0]);
            }
            return new KeyValue(fi.Name, null, fi.MemberType);
        }

        protected virtual object GetKeyValueDirect(object o)
        {
            return null;
        }

        public Dictionary<string, object> GetKeyValues(object o)
        {
            var dic = new Dictionary<string, object>();
            this.GetKeyValuesDirect(dic, o);
            return dic;
        }

        protected abstract void GetKeyValuesDirect(Dictionary<string, object> dic, object o);

        public void SetKeyValue(object obj, object key)
        {
            SetKeyValueDirect(obj, key);
        }

        protected abstract void SetKeyValueDirect(object obj, object key);

        protected object GetNullable(object o, int objType)
        {
            if (o == null || o == DBNull.Value)
            {
                return null;
            }
            switch (objType)
            {
                case 1:
                    return new Guid(o.ToString());

                case 2:
                    return Convert.ToBoolean(o);

                case 3:
                    return (Date) ((DateTime) o);

                case 4:
                    return (Time) ((DateTime) o);
            }
            return o;
        }

        public void Init(ObjectInfo oi)
        {
            var keys = new List<KeyValuePair<string, string>>();
            SetValuesForSelectDirectNoLazy(keys);
            var result = new List<KeyValue>();
            foreach(var key in keys)
            {
                var member = oi.Members.FirstOrDefault(p => p.Name == key.Key);
                if(!member.Is.DbGenerate)
                {
                    result.Add(GetKeyValue(oi, member));
                }
            }
            this.kvc = result.ToArray();
        }

        public void LoadRelationValues(object o, bool useIndex, bool noLazy, IDataReader dr)
        {
            if (useIndex)
            {
                if (noLazy)
                {
                    this.LoadRelationValuesByIndexNoLazy(o, dr);
                }
                else
                {
                    this.LoadRelationValuesByIndex(o, dr);
                }
            }
            else if (noLazy)
            {
                this.LoadRelationValuesByNameNoLazy(o, dr);
            }
            else
            {
                this.LoadRelationValuesByName(o, dr);
            }
        }

        protected abstract void LoadRelationValuesByIndex(object o, IDataReader dr);
        protected abstract void LoadRelationValuesByIndexNoLazy(object o, IDataReader dr);
        protected abstract void LoadRelationValuesByName(object o, IDataReader dr);
        protected abstract void LoadRelationValuesByNameNoLazy(object o, IDataReader dr);

        public void LoadSimpleValues(object o, bool useIndex, IDataReader dr)
        {
            if (useIndex)
            {
                this.LoadSimpleValuesByIndex(o, dr);
            }
            else
            {
                this.LoadSimpleValuesByName(o, dr);
            }
        }

        protected abstract void LoadSimpleValuesByIndex(object o, IDataReader dr);
        protected abstract void LoadSimpleValuesByName(object o, IDataReader dr);

        protected KeyOpValue NewKeyValue(int n, object v)
        {
            KeyValue value2 = this.kvc[n];
            return new KeyOpValue(value2.Key, v, value2.ValueType);
        }

        protected KeyOpValue NewSpKeyValueDirect(int n, int op)
        {
            KeyValue value2 = this.kvc[n];
            if(op == 1)
            {
                return new KeyOpValue(value2.Key, 1, KvOpertation.Add);
            }
            if(op == 2)
            {
                return new KeyOpValue(value2.Key, null, KvOpertation.Now);
            }
            throw new ApplicationException();
        }

        public void SetValuesForInsert(ISqlValues isv, object obj)
        {
            this.SetValuesForInsertDirect(isv.Values, obj);
        }

        protected abstract void SetValuesForInsertDirect(List<KeyOpValue> values, object o);

        public void SetValuesForSelect(ISqlKeys isv, bool noLazy)
        {
            if (noLazy)
            {
                this.SetValuesForSelectDirectNoLazy(isv.Keys);
            }
            else
            {
                this.SetValuesForSelectDirect(isv.Keys);
            }
        }

        protected abstract void SetValuesForSelectDirect(List<KeyValuePair<string, string>> keys);
        protected abstract void SetValuesForSelectDirectNoLazy(List<KeyValuePair<string, string>> keys);

        public void SetValuesForUpdate(ISqlValues isv, object obj)
        {
            this.SetValuesForUpdateDirect(isv.Values, obj);
        }

        protected abstract void SetValuesForUpdateDirect(List<KeyOpValue> values, object o);
    }
}

