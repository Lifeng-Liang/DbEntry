using System.Collections;
using System.Collections.Generic;
using Lephone.Core;
using Lephone.Data.Definition;
using Lephone.Data.Model;
using Lephone.Data.Model.Composer;
using Lephone.Data.Model.Handler;
using Lephone.Data.Model.Inserter;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Caching
{
    public class CachedModelOperator : ModelOperator
    {
        internal CachedModelOperator(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler)
            : base(info, composer, provider, handler)
        {
        }

        protected internal virtual void SetCachedObject(object obj)
        {
            string key = KeyGenerator.Instance[obj];
            if (Scope<ConnectionContext>.Current != null && Scope<ConnectionContext>.Current.IsInTransaction)
            {
                if (Scope<ConnectionContext>.Current.Jar == null)
                {
                    Scope<ConnectionContext>.Current.Jar = new Dictionary<string, object>();
                }
                Scope<ConnectionContext>.Current.Jar[key] = obj;
            }
            else
            {
                SetObjectToCache(key, obj);
            }
        }

        private static void SetObjectToCache(string key, object obj)
        {
            CacheProvider.Instance[key] = ModelContext.CloneObject(obj);
        }

        protected override object InnerGetObject(object key)
        {
            var sk = KeyGenerator.Instance.GetKey(Info.HandleType, key);

            object co = Scope<ConnectionContext>.Current != null 
                && Scope<ConnectionContext>.Current.Jar != null 
                && Scope<ConnectionContext>.Current.Jar.ContainsKey(sk) 
                            ? Scope<ConnectionContext>.Current.Jar[sk] 
                            : CacheProvider.Instance[sk];
            if (co != null)
            {
                object objInCache = ModelContext.CloneObject(co);
                return objInCache;
            }

            var obj = base.InnerGetObject(key);

            if (obj != null)
            {
                SetCachedObject(obj);
            }
            return obj;
        }

        public override void Save(IDbObject obj)
        {
            base.Save(obj);
            SetCachedObject(obj);
        }

        public override void Insert(IDbObject obj)
        {
            base.Insert(obj);
            SetCachedObject(obj);
        }

        public override void Update(IDbObject obj)
        {
            base.Update(obj);
            SetCachedObject(obj);
        }

        protected override IProcessor GetListProcessor(IList il)
        {
            var li = base.GetListProcessor(il);
            return new CachedListInserter(li);
        }

        public override int Delete(IDbObject obj)
        {
            var n = base.Delete(obj);
            CacheProvider.Instance.Remove(KeyGenerator.Instance[obj]);
            return n;
        }
    }
}
