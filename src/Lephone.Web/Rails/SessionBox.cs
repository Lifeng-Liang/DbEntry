using System;
using System.Collections.Generic;
using System.Web;

namespace Lephone.Web.Rails
{
    public class SessionBox
    {
        protected static Dictionary<string, Dictionary<string, object>> BagSet = new Dictionary<string, Dictionary<string, object>>();
        protected static DateTime NextChekTime = DateTime.MinValue;

        public virtual object this[string Name]
        {
            get
            {
                Dictionary<string, object> bag = GetCurrentBag();
                if (bag.ContainsKey(Name))
                {
                    return bag[Name];
                }
                return null;
            }
            set
            {
                Dictionary<string, object> bag = GetCurrentBag();
                bag[Name] = value;
            }
        }

        protected static Dictionary<string, object> GetCurrentBag()
        {
            ClearExpires();
            HttpContext ctx = HttpContext.Current;
            HttpCookie c = ctx.Request.Cookies["lf_session_id"];
            if (c != null)
            {
                Dictionary<string, object> bag = BagSet[c.Value];
                bag["ExpireTime"] = DateTime.Now.AddMinutes(WebSettings.SessionExpire);
                return bag;
            }
            string fid = Guid.NewGuid().ToString();
            HttpCookie rpc = ctx.Response.Cookies["lf_session_id"];
            if (rpc != null)
            {
                rpc.Value = fid;
                var bag = new Dictionary<string, object>();
                bag["ExpireTime"] = DateTime.Now.AddMinutes(WebSettings.SessionExpire);
                lock (BagSet)
                {
                    BagSet[fid] = bag;
                }
                return bag;
            }
            throw new WebException("Unexpacted exception");
        }

        protected static void ClearExpires()
        {
            DateTime now = DateTime.Now;
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
