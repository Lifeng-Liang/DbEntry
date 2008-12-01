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

        public void NoView()
        {
            bag["item"] = "Test for no view file!";
        }

        public void Say(string s1, string s2)
        {
            bag["item"] = string.Format("<b>{0}, {1}</b>", s1, s2);
        }

        public void Hello(string[] ss)
        {
            string s = "";
            foreach (var s1 in ss)
            {
                s += s1 + "<br />";
            }
            bag["item"] = s;
        }
    }
}
