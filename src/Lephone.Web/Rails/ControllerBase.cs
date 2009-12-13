using System;
using System.Collections;
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
        private static readonly string ErrorTemplate = ResourceHelper.ReadToEnd(typeof(ControllerBase), "Rails.Error.htm");

        protected internal HttpContext Ctx;
        internal readonly Dictionary<string, object> Bag = new Dictionary<string, object>();
        public readonly string ControllerName;
        public readonly FlashBox Flash = new FlashBox();
        public readonly SessionBox Session = new SessionBox();

        protected object this[string key]
        {
            get
            {
                return Bag[key];
            }
            set
            {
                Bag[key] = value;
            }
        }

        private UrlToInfo.UrlTo _urlTo;

        protected internal UrlToInfo.UrlTo UrlTo
        {
            get { return _urlTo ?? (_urlTo = new UrlToInfo.UrlTo(ControllerName)); }
        }

        protected ControllerBase()
        {
            string cn = GetType().Name;
            if (cn.EndsWith("Controller"))
            {
                cn = cn.Substring(0, cn.Length - 10);
            }
            ControllerName = cn.ToLower();
        }

        protected internal virtual string OnBeforeAction(string actionName)
        {
            return null;
        }

        protected internal virtual string OnAfterAction(string actionName)
        {
            return null;
        }

        protected internal virtual void OnException(Exception ex)
        {
            Exception e = ex.InnerException ?? ex;
            string title = Ctx.Server.HtmlEncode(e.Message);
            string text = e.ToString();
            string result = string.Format(ErrorTemplate, title, text);
            Ctx.Response.Write(result);
        }

        public void RedirectTo(string url)
        {
            Ctx.Response.Redirect(url);
        }
    }

    [Scaffolding]
    public abstract class ControllerBase<T> : ControllerBase where T : class, IDbObject
    {
        //public T Item
        //{
        //    set
        //    {
        //        this["Item"] = value;
        //    }
        //}

        //public IList Items
        //{
        //    set
        //    {
        //        this["List"] = value;
        //    }
        //}

        //public long ListCount
        //{
        //    set
        //    {
        //        this["ListCount"] = value;
        //    }
        //}

        //public int ListPageSize
        //{
        //    set
        //    {
        //        this["ListPageSize"] = value;
        //    }
        //}

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
                    string s = Ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
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
            Flash.Notice = string.Format("{0} was successfully created", ControllerName);
            RedirectTo(UrlTo.Action("list"));
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
            IPagedSelector ps = DbEntry.From<T>().Where(Condition.Empty).OrderBy("Id DESC")
                .PageSize(psize).GetPagedSelector();
            this["List"] = ps.GetCurrentPage(pageIndex);
            this["ListCount"] = ps.GetResultCount();
            this["ListPageSize"] = WebSettings.DefaultPageSize;
        }

        public virtual void Show(int n)
        {
            this["Item"] = DbEntry.GetObject<T>(n);
        }

        public virtual void Edit(int n)
        {
            this["Item"] = DbEntry.GetObject<T>(n);
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
                    string s = Ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
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
            Flash.Notice = string.Format("{0} was successfully updated", ControllerName);
            RedirectTo(UrlTo.Action("show").Parameters(n));
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
                RedirectTo(UrlTo.Action("list"));
            }
        }
    }
}
