
using System;
using Lephone.Web.Rails;

namespace DebugLib.Controllers
{
    public class TestController : ControllerBase
    {
        [DefaultAction]
        public void Add(int m, int n)
        {
            int r = m + n;
            bag["obj"] = r;
        }

        public void Date(DateTime dt, int n)
        {
            bag["obj"] = dt.AddDays(n);
        }
    }
}
