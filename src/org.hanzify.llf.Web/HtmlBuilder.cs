
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Lephone.Web
{
    public class HtmlBuilder
    {
        public static HtmlBuilder New
        {
            get { return new HtmlBuilder(); }
        }

        protected StringBuilder result = new StringBuilder();
        protected Stack<string> tags = new Stack<string>();
        protected string ctag = string.Empty;

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

        public HtmlBuilder text(string text)
        {
            return include(HttpUtility.HtmlEncode(text));
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

        public HtmlBuilder end
        {
            get
            {
                string s = tags.Pop();
                if (s == ctag)
                {
                    result.Length--;
                    result.Append(" />");
                }
                else
                {
                    result.Append("</").Append(s).Append(">");
                }
                ctag = string.Empty;
                return this;
            }
        }

        public HtmlBuilder enter
        {
            get { return include("\r\n"); }
        }

        public HtmlBuilder newline
        {
            get { return include("\n"); }
        }

        public HtmlBuilder include(HtmlBuilder hb)
        {
            return include(hb.ToString());
        }

        public HtmlBuilder include(string text)
        {
            result.Append(text);
            ctag = string.Empty;
            return this;
        }

        public HtmlBuilder tag(string TagName)
        {
            result.Append("<").Append(TagName).Append(">");
            tags.Push(TagName);
            ctag = TagName;
            return this;
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

        public HtmlBuilder id(string ID)
        {
            return attr("id", ID);
        }

        public HtmlBuilder attr(string Name, object Value)
        {
            return attr(Name, Value.ToString());
        }

        public HtmlBuilder attr(string Name, string Value)
        {
            if (result.Length > 0 && result[result.Length - 1] == '>')
            {
                result.Length--;
                result.Append(" ").Append(Name).Append("=\"").Append(Value).Append("\">");
                return this;
            }
            else
            {
                throw new WebException(string.Format("The attribute '{0}' can not be added because there is no tag before it.", Name));
            }
        }

        /// <summary>
        /// it is use as end point of code line and don't need set value mode.
        /// Example:
        /// b.end.over();  // Same as:  b = b.end;
        /// </summary>
        /// <returns>Instance of itself</returns>
        public HtmlBuilder over()
        {
            return this;
        }

        public override string ToString()
        {
            if (tags.Count != 0)
            {
                throw new WebException("There are some tags not closed!");
            }
            return result.ToString();
        }
    }
}
