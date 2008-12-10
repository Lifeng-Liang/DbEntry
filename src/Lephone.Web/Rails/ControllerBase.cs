using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public abstract class ControllerBase
    {
        protected internal HttpContext ctx;
        public readonly Dictionary<string, object> bag = new Dictionary<string, object>();
        public readonly string ControllerName;
        public readonly FlashBox Flash = new FlashBox();
        public readonly SessionBox Session = new SessionBox();

        protected ControllerBase()
        {
            string cn = GetType().Name;
            if (cn.EndsWith("Controller"))
            {
                cn = cn.Substring(0, cn.Length - 10);
            }
            ControllerName = cn.ToLower();
        }

        protected internal virtual string OnBeforeAction(string ActionName)
        {
            return null;
        }

        protected internal virtual string OnAfterAction(string ActionName)
        {
            return null;
        }

        protected internal virtual void OnException(Exception ex)
        {
            Exception e = ex.InnerException ?? ex;
            ctx.Response.Write(string.Format("<h1>{0}<h1>", ctx.Server.HtmlEncode(e.Message)));
        }

        public void RedirectTo(UTArgs args, params object[] paramters)
        {
            string url = UrlTo(args, paramters);
            ctx.Response.Redirect(url);
        }

        public string UrlTo(UTArgs args, params object[] paramters)
        {
            if (string.IsNullOrEmpty(args.Controller))
            {
                args.Controller = ControllerName;
            }
            return MasterPageBase.UrlTo(args.Controller, args.Action, paramters);
        }
    }

    [Scaffolding]
    public abstract class ControllerBase<T> : ControllerBase where T : class, IDbObject
    {
        public virtual void New()
        {
        }

        public virtual void Create()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            var obj = (T)oi.NewObject();
            foreach(MemberHandler m in oi.Fields)
            {
                if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                {
                    string s = ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
                    if (m.IsLazyLoad)
                    {
                        object ll = m.MemberInfo.GetValue(obj);
                        PropertyInfo pi = m.MemberInfo.MemberType.GetProperty("Value");
                        object v = ControllerHelper.ChangeType(s, m.FieldType.GetGenericArguments()[0]);
                        pi.SetValue(ll, v, null);
                    }
                    else
                    {
                        m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.FieldType));
                    }
                }
            }
            if(obj is DbObjectSmartUpdate)
            {
                (obj as DbObjectSmartUpdate).Save();
            }
            else
            {
                DbEntry.Save(obj);
            }
            Flash["notice"] = string.Format("{0} was successfully created", ControllerName);
            RedirectTo(new UTArgs{Action = "list"});
        }

        public virtual void List(int pageIndex, int? pageSize)
        {
            if (pageIndex < 0)
            {
                throw new DataException("The pageIndex out of supported range.");
            }
            if (pageIndex != 0)
            {
                pageIndex--;
            }
            int psize = pageSize ?? WebSettings.DefaultPageSize;
            IPagedSelector ps = DbEntry.From<T>().Where(WhereCondition.EmptyCondition).OrderBy("Id DESC")
                .PageSize(psize).GetPagedSelector();
            bag["list"] = ps.GetCurrentPage(pageIndex);
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
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            var obj = DbEntry.GetObject<T>(n);
            foreach (MemberHandler m in oi.Fields)
            {
                if (m.IsRelationField) { continue; }
                if (!m.IsAutoSavedValue && !m.IsDbGenerate)
                {
                    string s = ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
                    if (m.IsLazyLoad)
                    {
                        object ll = m.MemberInfo.GetValue(obj);
                        PropertyInfo pi = m.MemberInfo.MemberType.GetProperty("Value");
                        object v = ControllerHelper.ChangeType(s, m.FieldType.GetGenericArguments()[0]);
                        pi.SetValue(ll, v, null);
                        // TODO: get rid of use such method.
                        typeof (T).GetMethod("m_ColumnUpdated", ClassHelper.AllFlag).Invoke(obj, new object[] {m.Name});
                    }
                    else
                    {
                        m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.FieldType));
                    }
                }
            }
            if (obj is DbObjectSmartUpdate)
            {
                (obj as DbObjectSmartUpdate).Save();
            }
            else
            {
                DbEntry.Save(obj);
            }
            Flash["notice"] = string.Format("{0} was successfully updated", ControllerName);
            RedirectTo(new UTArgs{Action = "show"}, n);
        }

        public virtual void Destroy(int n)
        {
            object o = DbEntry.GetObject<T>(n);
            if (o != null)
            {
                if (o is DbObjectSmartUpdate)
                {
                    (o as DbObjectSmartUpdate).Delete();
                }
                else
                {
                    DbEntry.Save(o);
                }
                RedirectTo(new UTArgs{Action = "list"});
            }
        }
    }
}
