
using System;
using System.Collections.Generic;
using System.Web;

namespace Lephone.Web.Rails
{
    public class FlashBox
    {
        protected static Dictionary<string, Dictionary<string, object>> BagSet = new Dictionary<string,Dictionary<string,object>>();
        protected static DateTime NextChekTime = DateTime.MinValue;

        public FlashBox()
        {
        }

        public virtual object this[string Name]
        {
            get
            {
                Dictionary<string, object> bag = GetCurrentBag();
                if (bag.ContainsKey(Name))
                {
                    object o = bag[Name];
                    bag.Remove(Name);
                    return o;
                }
                return string.Empty;
            }
            set
            {
                Dictionary<string, object> bag = GetCurrentBag();
                bag[Name] = value;
            }
        }

        protected Dictionary<string, object> GetCurrentBag()
        {
            ClearExpires();
            HttpContext ctx = HttpContext.Current;
            HttpCookie c = ctx.Request.Cookies["flash_id"];
            if (c != null)
            {
                Dictionary<string, object> bag = BagSet[c.Value];
                bag["ExpireTime"] = DateTime.Now.AddMinutes(WebSettings.SessionExpire);
                return bag;
            }
            else
            {
                string fid = Guid.NewGuid().ToString();
                ctx.Response.Cookies["flash_id"].Value = fid;
                Dictionary<string, object> bag = new Dictionary<string, object>();
                bag["ExpireTime"] = DateTime.Now.AddMinutes(WebSettings.SessionExpire);
                lock (BagSet)
                {
                    BagSet[fid] = bag;
                }
                return bag;
            }
        }

        protected void ClearExpires()
        {
            DateTime now = DateTime.Now;
            if (now > NextChekTime)
            {
                NextChekTime = now.AddMinutes(WebSettings.SessionCheckEvery);
                List<string> keys = new List<string>(BagSet.Keys);
                foreach (string s in keys)
                {
                    DateTime et = (DateTime)BagSet[s]["ExpireTime"];
                    if (now > et)
                    {
                        BagSet.Remove(s);
                    }
                }
            }
        }
    }
}
