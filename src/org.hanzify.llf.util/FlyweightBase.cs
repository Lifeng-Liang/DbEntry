
using System;
using System.Collections.Generic;

namespace Lephone.Util
{
    public abstract class FlyweightBase<TKey, TValue> where TValue : FlyweightBase<TKey, TValue>
    {
        protected static Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

        public static TValue GetInstance(TKey t)
        {
            if (dic.ContainsKey(t))
            {
                return dic[t];
            }
            else
            {
                TValue v = ClassHelper.CreateInstance<TValue>();
                v.Init(t);
                lock (dic)
                {
                    dic[t] = v;
                }
                return v;
            }
        }

        protected abstract void Init(TKey t);
    }
}
