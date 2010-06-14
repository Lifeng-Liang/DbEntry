using System;
using System.Collections.Generic;
using Lephone.Core;

namespace Lephone.Web.Mvc
{
    public class SessionHandler
    {
        public class SessionItem
        {
            public long ExpireTime;
            public Dictionary<string, object> Bag;
        }

        protected static Dictionary<string, SessionItem> BagSet = new Dictionary<string, SessionItem>();

        private static readonly long SessionExpire = (long)WebSettings.SessionExpire * 60;
        private static readonly long SessionCheckEvery = (long)WebSettings.SessionCheckEvery * 60;

        private static long _nextChekTime;

        public static long NextCheckTime
        {
            get { return _nextChekTime; }
        }

        public virtual object this[string name]
        {
            get
            {
                var bag = GetCurrentBag();
                if (bag != null && bag.ContainsKey(name))
                {
                    return bag[name];
                }
                return null;
            }
            set
            {
                var bag = GetOrCreateCurrentBag();
                bag[name] = value;
            }
        }

        public virtual int Count
        {
            get
            {
                ClearExpires();
                return BagSet.Count;
            }
        }

        public virtual int CurrentCount
        {
            get
            {
                var bag = GetCurrentBag();
                return bag == null ? -1 : bag.Count;
            }
        }

        protected static Dictionary<string, object> GetOrCreateCurrentBag()
        {
            var bag =  GetCurrentBag();
            if(bag != null)
            {
                return bag;
            }
            string fid = Guid.NewGuid().ToString();
            CookiesHandler.Instance["lf_session_id"] = fid;
            var item = new SessionItem
                           {
                               ExpireTime = MiscProvider.Instance.Secends + SessionExpire,
                               Bag = new Dictionary<string, object>()
                           };
            lock (BagSet)
            {
                BagSet[fid] = item;
            }
            return item.Bag;
        }

        protected static Dictionary<string, object> GetCurrentBag()
        {
            ClearExpires();
            string cv = CookiesHandler.Instance["lf_session_id"];
            if (cv != null)
            {
                if(BagSet.ContainsKey(cv))
                {
                    var item = BagSet[cv];
                    if(item != null)
                    {
                        item.ExpireTime = MiscProvider.Instance.Secends + SessionExpire;
                        return item.Bag;
                    }
                }
            }
            return null;
        }

        protected static void ClearExpires()
        {
            var now = MiscProvider.Instance.Secends;
            if (now > _nextChekTime)
            {
                lock (BagSet)
                {
                    if (now > _nextChekTime)
                    {
                        _nextChekTime = now + SessionCheckEvery;
                        var keys = new List<string>(BagSet.Keys);
                        foreach (string s in keys)
                        {
                            var et = BagSet[s].ExpireTime;
                            if (now > et)
                            {
                                BagSet.Remove(s);
                            }
                        }
                    }
                }
            }
        }
    }
}


