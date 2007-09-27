
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Web.Common;

namespace Lephone.Web
{
    public class ControllerBase
    {
        protected internal HttpContext ctx;
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();

        public ControllerBase()
        {
        }
    }

    public class ControllerBase<T> : ControllerBase
    {
        private string GetControllerName()
        {
            string ControllerName = this.GetType().Name;
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
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            T obj = (T)oi.NewObject();
            foreach(MemberHandler m in oi.SimpleFields)
            {
                if (!m.IsKey)
                {
                    string s = ctx.Request.Form[ControllerName + "_" + m.MemberInfo.Name.ToLower()];
                    m.MemberInfo.SetValue(obj, Convert.ChangeType(s, m.FieldType));
                }
            }
            DbEntry.Save(obj);
            string url = PageBase.UrlTo(ctx.Request.ApplicationPath, ControllerName, "list", null);
            ctx.Response.Redirect(url);
        }

        public virtual void List(int PageIndex)
        {
            if (PageIndex < 0)
            {
                throw new DbEntryException("The PageIndex out of supported range.");
            }
            if (PageIndex != 0)
            {
                PageIndex--;
            }
            IPagedSelector ps = DbEntry.From<T>().Where(null).OrderBy("Id DESC")
                .PageSize(WebSettings.DefaultPageSize).GetPagedSelector();
            bag["list"] = ps.GetCurrentPage(PageIndex);
            bag["list_count"] = ps.GetResultCount();
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
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            T obj = (T)oi.NewObject();
            foreach (MemberHandler m in oi.SimpleFields)
            {
                if (m.IsKey)
                {
                    m.SetValue(obj, Convert.ChangeType(n, m.FieldType));
                }
                else
                {
                    string s = ctx.Request.Form[ControllerName + "_" + m.MemberInfo.Name.ToLower()];
                    m.MemberInfo.SetValue(obj, Convert.ChangeType(s, m.FieldType));
                }
            }
            DbEntry.Save(obj);
            string url = PageBase.UrlTo(ctx.Request.ApplicationPath, ControllerName, "show", n.ToString());
            ctx.Response.Redirect(url);
        }

        public virtual void Destroy(int n)
        {
            object o = DbEntry.GetObject<T>(n);
            if (o != null)
            {
                DbEntry.Delete(o);
                bag["work"] = true;
                string url = PageBase.UrlTo(ctx.Request.ApplicationPath, GetControllerName(), "list", null);
                ctx.Response.Redirect(url);
            }
            else
            {
                bag["work"] = false;
            }
        }
    }
}
