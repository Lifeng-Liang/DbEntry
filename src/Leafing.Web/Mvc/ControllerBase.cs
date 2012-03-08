using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Leafing.Web.Mvc.Core;

namespace Leafing.Web.Mvc
{
    public abstract class ControllerBase
    {
        internal readonly Dictionary<string, object> Bag = new Dictionary<string, object>();
        public readonly string ControllerName;
        public readonly FlashHandler Flash = new FlashHandler();
        public readonly CookiesHandler Cookies = CookiesHandler.Instance;

        public object this[string key]
        {
            [Exclude]
            get { return Bag[key]; }

            [Exclude]
            set { Bag[key] = value; }
        }

        [Exclude]
        public UrlToInfo UrlTo(string controllerName = null)
        {
            return LinkHelper.UrlTo(controllerName ?? ControllerName);
        }

        [Exclude]
        public UrlToInfo UrlTo<T>(Expression<Action<T>> expr = null) where T : ControllerBase
        {
            return LinkHelper.UrlTo(expr);
        }

        [Exclude]
        public T Bind<T>() where T : class 
        {
            return (T)TypeBinder.Instance.GetObject(null, typeof(T));
        }

        [Exclude]
        public T Bind<T>(string name) where T : struct
        {
            return (T)TypeBinder.Instance.GetObject(name, typeof(T));
        }

        [Exclude]
        public string Bind(string name)
        {
            return (string)TypeBinder.Instance.GetObject(name, typeof(string));
        }

        protected ControllerBase()
        {
            ControllerName = LinkHelper.GetControllerNameByType(GetType());
        }

        [Exclude]
        public void RedirectTo(string url)
        {
            HttpContextHandler.Instance.Redirect(url, false);
        }
    }
}


