using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class DefaultController : ControllerBase
    {
        private static readonly string template = ResourceHelper.ReadToEnd(typeof(DefaultController), "Rails.Default.htm");

        public void List()
        {
            HtmlBuilder b = new HtmlBuilder();
            foreach (string s in RailsDispatcher.ctls.Keys)
            {
                b = b.li.a(s).text(s).end.end;
            }
            ctx.Response.Write(string.Format(template, b));
        }
    }
}
