using System.Reflection;
using Leafing.Core;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.Data.Model.Member;
using Leafing.Web.Mvc.Core;

namespace Leafing.Web.Mvc
{
    [Scaffolding]
    public abstract class ControllerBase<T> : ControllerBase where T : class, IDbObject
    {
        protected T Item
        {
            set { this["Item"] = value; }
        }

        protected ItemList<T> ItemList
        {
            set { this["ItemList"] = value; }
        }

        public virtual void New()
        {
        }

        public virtual string Create()
        {
            var ctx = ModelContext.GetInstance(typeof(T));
            var obj = (T)ctx.NewObject();
            foreach (MemberHandler m in ctx.Info.Members)
            {
                if (!m.Is.RelationField && !m.Is.DbGenerate && !m.Is.AutoSavedValue)
                {
                    string s = HttpContextHandler.Instance[ControllerName + "[" + m.Name.ToLower() + "]"];
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
            if (obj is DbObjectSmartUpdate)
            {
                (obj as DbObjectSmartUpdate).Save();
            }
            else
            {
                DbEntry.Save(obj);
            }
            Flash.Notice = string.Format("{0} was successfully created", ControllerName);
            return UrlTo().Action("list");
        }

        public virtual void List(long? pageIndex, int? pageSize)
        {
            var style = ControllerFinder.Controllers[ControllerName].ListStyle;
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
                    string s = HttpContextHandler.Instance[ControllerName + "[" + m.Name.ToLower() + "]"];
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
            return UrlTo().Action("show").Parameters(n);
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
                return UrlTo().Action("list");
            }
            return null;
        }
    }
}
