namespace Lephone.Web
{
    public class HtmlBuilder : XmlBuilder<HtmlBuilder>
    {
        public static HtmlBuilder New
        {
            get { return new HtmlBuilder(); }
        }

        public HtmlBuilder form(string method, string action)
        {
            return tag("form").attr("action", action).attr("method", method);
        }

        public HtmlBuilder name(object name)
        {
            return attr("name", name);
        }

        public HtmlBuilder type(object type)
        {
            return attr("type", type);
        }

        public HtmlBuilder value(object value)
        {
            return attr("value", value);
        }

        public HtmlBuilder p
        {
            get { return tag("p"); }
        }

        public HtmlBuilder h1
        {
            get { return tag("h1"); }
        }

        public HtmlBuilder label
        {
            get { return tag("label"); }
        }

        public HtmlBuilder input
        {
            get { return tag("input"); }
        }

        public HtmlBuilder br
        {
            get { return include("<br />"); }
        }

        public HtmlBuilder hr
        {
            get { return include("<hr />"); }
        }

        public HtmlBuilder table
        {
            get { return tag("table"); }
        }

        public HtmlBuilder th
        {
            get { return tag("th"); }
        }

        public HtmlBuilder tr
        {
            get { return tag("tr"); }
        }

        public HtmlBuilder td
        {
            get { return tag("td"); }
        }

        public HtmlBuilder a(string href)
        {
            return tag("a").attr("href", href);
        }

        public HtmlBuilder img(string src)
        {
            return tag("img").attr("src", src).end;
        }

        public HtmlBuilder img(string src, string alt, int height, int width)
        {
            return tag("img").attr("src", src).attr("alt", alt).attr("height", height).attr("width", width).end;
        }

        public HtmlBuilder ul
        {
            get { return tag("ul"); }
        }

        public HtmlBuilder ol
        {
            get { return tag("ol"); }
        }

        public HtmlBuilder li
        {
            get { return tag("li"); }
        }

        public HtmlBuilder div
        {
            get { return tag("div"); }
        }

        public HtmlBuilder span
        {
            get { return tag("span"); }
        }

        public HtmlBuilder html
        {
            get { return tag("html"); }
        }

        public HtmlBuilder head
        {
            get { return tag("head"); }
        }

        public HtmlBuilder title
        {
            get { return tag("title"); }
        }

        public HtmlBuilder body
        {
            get { return tag("body"); }
        }

        public HtmlBuilder asp(string Name, string ID)
        {
            return tag("asp:" + Name).attr("ID", ID).attr("runat", "server");
        }

        public HtmlBuilder Class(string CssClass)
        {
            return attr("class", CssClass);
        }

        public HtmlBuilder style(string style)
        {
            return attr("style", style);
        }

        public HtmlBuilder id(string ID)
        {
            return attr("id", ID);
        }
    }
}
