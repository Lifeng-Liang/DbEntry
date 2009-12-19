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
        protected internal HttpContext Ctx;
        internal readonly Dictionary<string, object> Bag = new Dictionary<string, object>();
        public readonly string ControllerName;
        public readonly FlashHandler Flash = new FlashHandler();
        public readonly SessionHandler Session = new SessionHandler();

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
            ControllerHelper.OnException(ex, Ctx);
        }

        public void RedirectTo(string url)
        {
            Ctx.Response.Redirect(url, false);
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
            var isStatic = ControllerInfo.GetInstance(this.GetType()).IsStaticList;
            ProcessList(pageIndex, pageSize, isStatic);
        }

        protected void ProcessList(int pageIndex, int? pageSize, bool isStatic)
        {
            if (pageIndex < 0)
            {
                throw new DataException("The pageIndex out of supported range.");
            }
            int psize = pageSize ?? WebSettings.DefaultPageSize;
            var psd = DbEntry.From<T>().Where(Condition.Empty).OrderBy("Id DESC").PageSize(psize);
            var ps = isStatic ? psd.GetStaticPagedSelector() : psd.GetPagedSelector();


            var listCount = ps.GetResultCount();
            var listPageCount = ps.GetPageCount();
            if(pageIndex == 0)
            {
                if(isStatic)
                {
                    pageIndex = (int)listPageCount;
                }
            }
            if (pageIndex != 0)
            {
                pageIndex--;
            }
            this["List"] = ps.GetCurrentPage(pageIndex);
            this["ListCount"] = listCount;
            this["ListPageSize"] = WebSettings.DefaultPageSize;
            this["ListPageCount"] = listPageCount;
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
