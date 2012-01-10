using Leafing.Core;

namespace Leafing.Web.Mvc.Core
{
    public class DefaultController : ControllerBase
    {
        private static readonly string Template = ResourceHelper.ReadToEnd(typeof(DefaultController), "Mvc.Core.Default.htm");

        public void List()
        {
            var b = HtmlBuilder.New;
            foreach (string s in ControllerFinder.Controllers.Keys)
            {
                string url = UrlTo(s);
                b = b.li.a(url).text(s).end.end;
            }
            HttpContextHandler.Instance.Write(string.Format(Template, b));
        }
    }
}


