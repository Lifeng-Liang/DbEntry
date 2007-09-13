
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
using Lephone.Util;

namespace Lephone.Web
{
    public class ClearHttpHandler : IHttpHandler
    {
        private static Dictionary<string, Type> ctls = new Dictionary<string, Type>();

        static ClearHttpHandler()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                string s = a.FullName.Split(',')[0];
                if (s != "Lephone.Data" && s != "Lephone.Util" && s != "Lephone.Web")
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(ControllerBase)))
                        {
                            string tn = t.Name;
                            if (tn.EndsWith("Controller"))
                            {
                                tn.Substring(0, tn.Length - 10);
                            }
                            ctls.Add(tn, t);
                        }
                    }
                }
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string url = context.Request.RawUrl;
            url = url.Substring(context.Request.ApplicationPath.Length);
            
            if (url.ToLower().EndsWith(".aspx"))
            {
                url = url.Substring(0, url.Length - 4);
            }

            string[] ss = url.Split(new char[]{'/'},  StringSplitOptions.RemoveEmptyEntries);

            if(ss.Length== 0)
            {
                throw new ArgumentOutOfRangeException("There is no paramters.");
            }
            if (ss.Length > 2)
            {
                throw new NotSupportedException("Not supported more than 2 paramters now.");
            }

            if (ctls.ContainsKey(ss[0]))
            {
                string op = ss.Length > 1 ? ss[1] : "list";
                Type t = ctls[ss[0]];
                ControllerBase ctl = ClassHelper.CreateInstance(t) as ControllerBase;
                ctl.context = context;
                MethodInfo mi = t.GetMethod(op, ClassHelper.AllFlag);
                mi.Invoke(ctl, new object[] { });
                return;
            }
            context.Response.StatusCode = 404;
            context.Response.Write("requested url is empty.");
        }
    }
}
