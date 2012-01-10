using System;
using System.Collections.Generic;
using System.Web;
using Leafing.Core.Ioc;

namespace Leafing.Web.Mvc.Core
{
    [DependenceEntry, Implementation(1)]
    public class HttpContextHandler
    {
        public static readonly HttpContextHandler Instance = SimpleContainer.Get<HttpContextHandler>();

        public virtual string this[string name]
        {
            get { return HttpContext.Current.Request[name]; }
        }

        public virtual Uri UrlReferrer
        {
            get { return HttpContext.Current.Request.UrlReferrer; }
        }

        public virtual string RawUrl
        {
            get { return HttpContext.Current.Request.RawUrl; }
        }

        public virtual string ApplicationPath
        {
            get { return HttpContext.Current.Request.ApplicationPath; }
        }

        public virtual string[] GetAllKeys()
        {
            var list = new List<string>(HttpContext.Current.Request.QueryString.AllKeys);
            list.AddRange(HttpContext.Current.Request.Form.AllKeys);
            return list.ToArray();
        }

        public virtual void Write(string s)
        {
            HttpContext.Current.Response.Write(s);
        }

        public virtual void Redirect(string url)
        {
            HttpContext.Current.Response.Redirect(url);
        }

        public virtual void Redirect(string url, bool endResponse)
        {
            HttpContext.Current.Response.Redirect(url, endResponse);
        }

        public virtual int StatusCode
        {
            get { return HttpContext.Current.Response.StatusCode; }
            set { HttpContext.Current.Response.StatusCode = value; }
        }

        public virtual string AppRelativeCurrentExecutionFilePath
        {
            get { return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath; }
        }
    }
}
