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
            lock (dic)
            {
                if (dic.ContainsKey(t))
                {
                    return dic[t];
                }
                var v = ClassHelper.CreateInstance<TValue>();
                TKey x = v.CheckKey(t);
                v.Init(x);
                dic[t] = v;
                return v;
            }
        }

        protected virtual  TKey CheckKey(TKey t)
        {
            return t;
        }

        protected abstract void Init(TKey t);
    }
}
