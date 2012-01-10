using System;
using System.IO;
using System.Web;
using System.Web.Compilation;

namespace Leafing.Web.Mvc.Core
{
    public class PageView : IView
    {
        protected HttpContext Context;
        protected string ViewName;
        protected ControllerBase Controller;
        protected ControllerInfo ControllerInfo;

        public PageView(string viewName, ControllerBase controller, ControllerInfo controllerInfo)
        {
            this.Context = HttpContext.Current;
            this.ViewName = viewName;
            this.Controller = controller;
            this.ControllerInfo = controllerInfo;
        }

        public virtual void Render()
        {
            PageBase p = CreatePage();
            if (p != null)
            {
                InitViewPage(p);
                ((IHttpHandler)p).ProcessRequest(Context);
            }
        }

        protected virtual PageBase CreatePage()
        {
            string vp = "/Views/" + ControllerInfo.Name + "/" + ViewName + ".aspx";
            if (Context.Request.ApplicationPath != "/")
            {
                vp = Context.Request.ApplicationPath + vp;
            }
            string pp = Context.Server.MapPath(vp);
            if (File.Exists(pp))
            {
                object o = BuildManager.CreateInstanceFromVirtualPath(vp, typeof(object));
                if (o == null || !(o is PageBase))
                {
                    throw new WebException("The template page must inherits from PageBase!!!");
                }
                return (PageBase)o;
            }
            if (ControllerInfo.IsScaffolding)
            {
                Type tt = GetScaffoldingType(ControllerInfo.Type);
                if (string.IsNullOrEmpty(WebSettings.ScaffoldingMasterPage))
                {
                    return new ScaffoldingViews(ControllerInfo, tt, Context);
                }
                return new ScaffoldingViewsWithMaster(ControllerInfo, tt, Context);
            }
            if (ControllerInfo.Type == typeof(DefaultController))
            {
                return null;
            }
            throw new WebException(string.Format("The action {0} doesn't have view file!!!", ViewName));
        }

        protected virtual void InitViewPage(PageBase p)
        {
            p.Bag = Controller.Bag;
            p.ControllerName = ControllerInfo.Name.ToLower();
            p.ActionName = ViewName.ToLower();
            p.InitFields();
        }

        protected virtual Type GetScaffoldingType(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ControllerBase<>))
            {
                return t.GetGenericArguments()[0];
            }
            if (t.BaseType == typeof(object))
            {
                throw new WebException("System Error!");
            }
            return GetScaffoldingType(t.BaseType);
        }
    }
}
