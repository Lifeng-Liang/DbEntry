
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Lephone.Web
{
    public class PageBase : Page
    {
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();

        public PageBase()
        {
        }
    }
}
