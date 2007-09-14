
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Lephone.Data;

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

        public virtual void List()
        {
            bag["list"] = DbEntry.From<T>().Where(null).OrderBy("Id DESC").Select();
        }

        public virtual void Edit()
        {
        }

        public virtual void Destroy()
        {
        }
    }
}
