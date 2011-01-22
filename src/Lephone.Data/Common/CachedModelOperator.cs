using System.Collections;
using System.Collections.Generic;
using Lephone.Core;
using Lephone.Data.Caching;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Common
{
    public class CachedModelOperator : ModelOperator
    {
        internal CachedModelOperator(ObjectInfo info, QueryComposer composer)
            : base(info, composer)
        {
        }

        protected internal override void OnBeginTransaction()
        {
            Scope<ConnectionContext>.Current.Jar = new List<string>();
        }

        protected internal override void OnCommittedTransaction()
        {
            Scope<ConnectionContext>.Current.Jar = null;
        }

        protected internal override void OnTransactionError()
        {
            if (DataSettings.CacheClearWhenError)
            {
                CacheProvider.Instance.Clear();
            }
            else
            {
                var keyList = (List<string>)Scope<ConnectionContext>.Current.Jar;
                foreach (string key in keyList)
                {
                    CacheProvider.Instance.Remove(key);
                }
            }
        }

        protected internal virtual void SetCachedObject(object obj)
        {
            string key = KeyGenerator.Instance[obj];
            CacheProvider.Instance[key] = ModelContext.CloneObject(obj);
            if (Scope<ConnectionContext>.Current != null)
            {
                if (Scope<ConnectionContext>.Current.Jar != null)
                {
                    var keyList = (List<string>)Scope<ConnectionContext>.Current.Jar;
                    keyList.Add(key);
                }
            }
        }

        protected override object InnerGetObject(object key)
        {
            object co = CacheProvider.Instance[KeyGenerator.Instance.GetKey(Info.HandleType, key)];
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
    }
}
