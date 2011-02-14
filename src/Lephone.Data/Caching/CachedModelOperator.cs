using System.Collections;
using System.Collections.Generic;
using Lephone.Core;
using Lephone.Data.Model;
using Lephone.Data.Model.Composer;
using Lephone.Data.Model.Inserter;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Caching
{
    public class CachedModelOperator : ModelOperator
    {
        internal CachedModelOperator(ObjectInfo info, QueryComposer composer, DataProvider provider)
            : base(info, composer, provider)
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
                Scope<ConnectionContext>.Current.Jar.Add(key, obj);
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

        protected override void InnerInsert(object obj)
        {
            base.InnerInsert(obj);
            SetCachedObject(obj);
        }

        protected override void InnerUpdate(object obj, Condition iwc, ModelContext ctx, DataProvider dp)
        {
            base.InnerUpdate(obj, iwc, ctx, dp);
            SetCachedObject(obj);
        }

        protected override IProcessor GetListProcessor(IList il)
        {
            var li = base.GetListProcessor(il);
            return new CachedListInserter(li);
        }

        public override int Delete(Definition.IDbObject obj)
        {
            var deleter = new CachedModelDeleter(obj);
            return deleter.Process();
        }
    }
}
