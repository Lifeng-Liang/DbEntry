using System.Collections.Generic;

namespace Leafing.Core {
    public abstract class FlyweightBase<TKey, TValue> {
        protected Dictionary<TKey, TValue> Jar = new Dictionary<TKey, TValue>();

        public TValue GetInstance(TKey t) {
            return GetInst(t);
        }

        protected virtual TValue GetInst(TKey tk) {
            var t = CheckKey(tk);
            if (Jar.ContainsKey(t)) {
                return Jar[t];
            }
            lock (Jar) {
                if (Jar.ContainsKey(t)) {
                    return Jar[t];
                }
                var v = CreateInst(t);
                Jar[t] = v;
                return v;
            }
        }

        protected virtual TKey CheckKey(TKey t) {
            return t;
        }

        protected abstract TValue CreateInst(TKey t);
    }
}
