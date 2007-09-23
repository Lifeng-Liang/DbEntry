
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Lephone.Data;
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
        public virtual void New()
        {
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

        public virtual void Edit(int n)
        {
        }

        public virtual void Destroy(int n)
        {
            object o = DbEntry.GetObject<T>(n);
            if (o != null)
            {
                DbEntry.Delete(o);
                bag["work"] = true;
            }
            else
            {
                bag["work"] = false;
            }
        }
    }
}
