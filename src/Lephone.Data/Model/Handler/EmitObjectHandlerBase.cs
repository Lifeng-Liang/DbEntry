using System;
using System.Collections.Generic;
using System.Data;
using Lephone.Data.Builder;
using Lephone.Data.Definition;
using Lephone.Data.Model.Member;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model.Handler
{
    public abstract class EmitObjectHandlerBase : IDbObjectHandler
    {
        protected KeyValue[] kvc;

        protected void AddKeyValue(KeyValueCollection values, DbObjectSmartUpdate o, string key, int n, object v)
        {
            if ((o.m_UpdateColumns != null) && o.m_UpdateColumns.ContainsKey(key))
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

        protected object GetNullable(object o, int objType)
        {
            if (o == DBNull.Value)
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
            var list = new List<KeyValue>();
            foreach (MemberHandler member in oi.Members)
            {
                if ((!member.Is.DbGenerate && !member.Is.HasOne) && (!member.Is.HasMany && !member.Is.HasAndBelongsToMany))
                {
                    list.Add(GetKeyValue(oi, member));
                }
            }
            this.kvc = list.ToArray();
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

        protected KeyValue NewKeyValue(int n, object v)
        {
            KeyValue value2 = this.kvc[n];
            return new KeyValue(value2.Key, v, value2.ValueType);
        }

        protected KeyValue NewKeyValueDirect(int n, object v)
        {
            KeyValue value2 = this.kvc[n];
            return new KeyValue(value2.Key, v, v.GetType());
        }

        public void SetValuesForInsert(ISqlValues isv, object obj)
        {
            this.SetValuesForInsertDirect(isv.Values, obj);
        }

        protected abstract void SetValuesForInsertDirect(KeyValueCollection values, object o);

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

        protected abstract void SetValuesForUpdateDirect(KeyValueCollection values, object o);
    }
}

