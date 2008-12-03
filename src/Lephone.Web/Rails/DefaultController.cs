using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class DefaultController : ControllerBase
    {
        private static readonly string template = ResourceHelper.ReadToEnd(typeof(DefaultController), "Rails.Default.htm");

        public void List()
        {
            var b = HtmlBuilder.New;
            foreach (string s in RailsDispatcher.ctls.Keys)
            {
                string url = UrlTo(new UTArgs {Controller = s});
                b = b.li.a(url).text(s).end.end;
            }
            ctx.Response.Write(string.Format(template, b));
        }
    }
}
