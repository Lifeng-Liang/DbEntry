using System;
using System.Collections.Generic;
using Lephone.Util;

namespace Lephone.Web.Mvc
{
    public class SessionHandler
    {
        protected static Dictionary<string, Dictionary<string, object>> BagSet = new Dictionary<string, Dictionary<string, object>>();
        protected static DateTime NextChekTime = DateTime.MinValue;

        public virtual object this[string name]
        {
            get
            {
                Dictionary<string, object> bag = GetCurrentBag();
                if (bag.ContainsKey(name))
                {
                    return bag[name];
                }
                return null;
            }
            set
            {
                Dictionary<string, object> bag = GetCurrentBag();
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
                return GetCurrentBag().Count;
            }
        }

        protected static Dictionary<string, object> GetCurrentBag()
        {
            ClearExpires();
            string cv = CookiesHandler.Instance["lf_session_id"];
            if (cv != null)
            {
                if(BagSet.ContainsKey(cv))
                {
                    Dictionary<string, object> bag = BagSet[cv];
                    bag["ExpireTime"] = MiscProvider.Instance.Now.AddMinutes(WebSettings.SessionExpire);
                    return bag;
                }
            }
            string fid = Guid.NewGuid().ToString();
            CookiesHandler.Instance["lf_session_id"] = fid;
            var bag2 = new Dictionary<string, object>();
            bag2["ExpireTime"] = MiscProvider.Instance.Now.AddMinutes(WebSettings.SessionExpire);
            lock (BagSet)
            {
                BagSet[fid] = bag2;
            }
            return bag2;
        }

        protected static void ClearExpires()
        {
            DateTime now = MiscProvider.Instance.Now;
            if (now > NextChekTime)
            {
                lock (BagSet)
                {
                    if (now > NextChekTime)
                    {
                        NextChekTime = now.AddMinutes(WebSettings.SessionCheckEvery);
                        var keys = new List<string>(BagSet.Keys);
                        foreach (string s in keys)
                        {
                            var et = (DateTime)BagSet[s]["ExpireTime"];
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


