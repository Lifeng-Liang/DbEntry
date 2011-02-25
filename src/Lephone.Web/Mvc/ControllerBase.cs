﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Core;
using Lephone.Data.Model.Member;

namespace Lephone.Web.Mvc
{
    public abstract class ControllerBase
    {
        public HttpContext Ctx { get; internal set; }
        internal readonly Dictionary<string, object> Bag = new Dictionary<string, object>();
        public readonly string ControllerName;
        public readonly FlashHandler Flash = new FlashHandler();
        public readonly SessionHandler Session = new SessionHandler();

        protected internal object this[string key]
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

        protected internal UrlToInfo UrlTo
        {
            get { return new UrlToInfo(ControllerName); }
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
        protected T Item
        {
            set
            {
                this["Item"] = value;
            }
        }

        protected ItemList<T> ItemList
        {
            set
            {
                this["ItemList"] = value;
            }
        }

        public virtual void New()
        {
        }

        public virtual string Create()
        {
            var ctx = ModelContext.GetInstance(typeof(T));
            var obj = (T)ctx.NewObject();
            foreach(MemberHandler m in ctx.Info.Members)
            {
                if (!m.Is.RelationField && !m.Is.DbGenerate && !m.Is.AutoSavedValue)
                {
                    string s = Ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
                    if (m.Is.LazyLoad)
                    {
                        object ll = m.MemberInfo.GetValue(obj);
                        PropertyInfo pi = m.MemberInfo.MemberType.GetProperty("Value");
                        object v = ControllerHelper.ChangeType(s, m.MemberType.GetGenericArguments()[0]);
                        pi.SetValue(ll, v, null);
                    }
                    else
                    {
                        m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.MemberType));
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
            return UrlTo.Action("list");
        }

        public virtual void List(long? pageIndex, int? pageSize)
        {
            var style = MvcDispatcher.Ctls[ControllerName].ListStyle;
            ProcessList(pageIndex, pageSize, style);
        }

        protected void ProcessList(long? pageIndex, int? pageSize, ListStyle style)
        {
            if (pageIndex < 0)
            {
                throw new DataException("The pageIndex out of supported range.");
            }
            int psize = pageSize ?? WebSettings.DefaultPageSize;
            var psd = DbEntry.From<T>().Where(Condition.Empty).OrderBy("Id DESC").PageSize(psize);
            this["ItemList"] = psd.GetItemList(style, pageIndex);
        }

        public virtual void Show(long n)
        {
            this["Item"] = DbEntry.GetObject<T>(n);
        }

        public virtual void Edit(long n)
        {
            this["Item"] = DbEntry.GetObject<T>(n);
        }

        public virtual string Update(long n)
        {
            var ctx = ModelContext.GetInstance(typeof(T));
            var obj = DbEntry.GetObject<T>(n);
            foreach (MemberHandler m in ctx.Info.Members)
            {
                if (m.Is.RelationField) { continue; }
                if (!m.Is.AutoSavedValue && !m.Is.DbGenerate)
                {
                    string s = Ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
                    if (m.Is.LazyLoad)
                    {
                        object ll = m.MemberInfo.GetValue(obj);
                        PropertyInfo pi = m.MemberInfo.MemberType.GetProperty("Value");
                        object v = ControllerHelper.ChangeType(s, m.MemberType.GetGenericArguments()[0]);
                        pi.SetValue(ll, v, null);
                        // TODO: get rid of use such method.
                        ClassHelper.CallFunction(obj, "m_ColumnUpdated", m.Name);
                    }
                    else
                    {
                        m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.MemberType));
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
            return UrlTo.Action("show").Parameters(n);
        }

        public virtual string Destroy(long n)
        {
            IDbObject o = DbEntry.GetObject<T>(n);
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
                return UrlTo.Action("list");
            }
            return null;
        }
    }
}


