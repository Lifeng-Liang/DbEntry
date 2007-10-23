
using System;
using Lephone.Web.Rails;

namespace DebugLib.Controllers
{
    [DefaultAction("Add")]
    public class TestController : ControllerBase
    {
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
