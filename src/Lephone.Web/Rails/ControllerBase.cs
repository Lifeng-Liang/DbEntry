using System;
using System.Collections.Generic;
using System.Web;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class ControllerBase
    {
        protected internal HttpContext ctx;
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();

        public ControllerBase()
        {
        }

        protected internal virtual void OnBeforeAction(string ActionName)
        {
        }

        protected internal virtual void OnAfterAction(string ActionName)
        {
        }

        protected internal virtual void OnException(Exception ex)
        {
            Exception e = ex.InnerException ?? ex;
            ctx.Response.Write(string.Format("<h1>{0}<h1>", ctx.Server.HtmlEncode(e.Message)));
        }
    }

    [Scaffolding]
    public class ControllerBase<T> : ControllerBase where T : class, IDbObject
    {
        protected FlashBox flash = new FlashBox();

        protected void RedirectTo(string Controller, string Action, object Paramter)
        {
            string ParamterStr = (Paramter == null) ? null : Paramter.ToString();
            string ControllerName = string.IsNullOrEmpty(Controller) ? GetControllerName() : Controller;
            string url = PageBase.UrlTo(ctx.Request.ApplicationPath, ControllerName, Action, ParamterStr);
            ctx.Response.Redirect(url);
        }

        private string GetControllerName()
        {
            string ControllerName = GetType().Name;
            if (ControllerName.EndsWith("Controller"))
            {
                ControllerName = ControllerName.Substring(0, ControllerName.Length - 10);
            }
            return ControllerName.ToLower();
        }

        public virtual void New()
        {
        }

        public virtual void Create()
        {
            string ControllerName = GetControllerName();
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            var obj = (T)oi.NewObject();
            foreach(MemberHandler m in oi.SimpleFields)
            {
                if (!m.IsDbGenerate && !m.IsAutoSavedValue)
                {
                    string s = ctx.Request.Form[ControllerName + "[" + m.MemberInfo.Name.ToLower() + "]"];
                    m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.FieldType));
                }
            }
            DbEntry.Save(obj);
            flash["notice"] = string.Format("{0} was successfully created", ControllerName);
            RedirectTo(null, "list", null);
        }

        public virtual void List(int PageIndex)
        {
            if (PageIndex < 0)
            {
                throw new DataException("The PageIndex out of supported range.");
            }
            if (PageIndex != 0)
            {
                PageIndex--;
            }
            IPagedSelector ps = DbEntry.From<T>().Where(WhereCondition.EmptyCondition).OrderBy("Id DESC")
                .PageSize(WebSettings.DefaultPageSize).GetPagedSelector();
            bag["list"] = ps.GetCurrentPage(PageIndex);
            bag["list_count"] = ps.GetResultCount();
            bag["list_pagesize"] = WebSettings.DefaultPageSize;
        }

        public virtual void Show(int n)
        {
            bag["item"] = DbEntry.GetObject<T>(n);
        }

        public virtual void Edit(int n)
        {
            bag["item"] = DbEntry.GetObject<T>(n);
        }

        public virtual void Update(int n)
        {
            string ControllerName = GetControllerName();
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            var obj = (T)oi.NewObject();
            foreach (MemberHandler m in oi.SimpleFields)
            {
                if (m.IsDbGenerate)
                {
                    m.SetValue(obj, ClassHelper.ChangeType(n, m.FieldType));
                }
                else if (!m.IsAutoSavedValue)
                {
                    string s = ctx.Request.Form[ControllerName + "[" + m.MemberInfo.Name.ToLower() + "]"];
                    m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.FieldType));
                }
            }
            DbEntry.Save(obj);
            flash["notice"] = string.Format("{0} was successfully updated", ControllerName);
            RedirectTo(null, "show", n);
        }

        public virtual void Destroy(int n)
        {
            object o = DbEntry.GetObject<T>(n);
            if (o != null)
            {
                DbEntry.Delete(o);
                RedirectTo(null, "list", null);
            }
        }
    }
}
