using System.Collections.Generic;

namespace Lephone.Core
{
    public abstract class FlyweightBase<TKey, TValue> where TValue : FlyweightBase<TKey, TValue>
    {
        protected static Dictionary<TKey, TValue> Jar = new Dictionary<TKey, TValue>();

        private static readonly TValue Instance = ClassHelper.CreateInstance<TValue>();

        public static TValue GetInstance(TKey t)
        {
            return Instance.GetInst(t);
        }

        protected virtual TValue GetInst(TKey tk)
        {
            var t = CheckKey(tk);
            if (Jar.ContainsKey(t))
            {
                return Jar[t];
            }
            lock (Jar)
            {
                if (Jar.ContainsKey(t))
                {
                    return Jar[t];
                }
                var v = CreateInst(t);
                Jar[t] = v;
                return v;
            }
        }

        protected virtual  TKey CheckKey(TKey t)
        {
            return t;
        }

        protected abstract TValue CreateInst(TKey t);
    }
}
