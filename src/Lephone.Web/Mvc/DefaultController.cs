using Lephone.Util;

namespace Lephone.Web.Mvc
{
    public class DefaultController : ControllerBase
    {
        private static readonly string Template = ResourceHelper.ReadToEnd(typeof(DefaultController), "Mvc.Default.htm");

        public void List()
        {
            var b = HtmlBuilder.New;
            foreach (string s in MvcDispatcher.Ctls.Keys)
            {
                string url = UrlTo.Controller(s);
                b = b.li.a(url).text(s).end.end;
            }
            Ctx.Response.Write(string.Format(Template, b));
        }
    }
}


