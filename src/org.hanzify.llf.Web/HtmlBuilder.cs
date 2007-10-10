
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Lephone.Web
{
    // It's not fully support for html.
    public class HtmlBuilder
    {
        public static HtmlBuilder New
        {
            get
            {
                return new HtmlBuilder();
            }
        }

        protected StringBuilder result = new StringBuilder();
        protected Stack<string> tags = new Stack<string>();

        public HtmlBuilder br
        {
            get
            {
                result.Append("<br />");
                return this;
            }
        }

        public HtmlBuilder hr
        {
            get
            {
                result.Append("<hr />");
                return this;
            }
        }

        public HtmlBuilder table
        {
            get
            {
                result.Append("<table>");
                tags.Push("table");
                return this;
            }
        }

        public HtmlBuilder th
        {
            get
            {
                result.Append("<th>");
                tags.Push("th");
                return this;
            }
        }

        public HtmlBuilder tr
        {
            get
            {
                result.Append("<tr>");
                tags.Push("tr");
                return this;
            }
        }

        public HtmlBuilder td
        {
            get
            {
                result.Append("<td>");
                tags.Push("td");
                return this;
            }
        }

        public HtmlBuilder text(string text)
        {
            result.Append(HttpUtility.HtmlEncode(text));
            return this;
        }

        public HtmlBuilder a(string href)
        {
            result.Append("<a href=\"").Append(href).Append("\">");
            tags.Push("a");
            return this;
        }

        public HtmlBuilder img(string src)
        {
            result.Append("<img src=\"").Append(src).Append("\" />");
            return this;
        }

        public HtmlBuilder img(string src, string alt, int height, int width)
        {
            result.Append("<img src=\"").Append(src).Append("\" alt=\"").Append(alt)
                .Append("\" height=\"").Append(height).Append("\" width=\"").Append(width).Append("\" />");
            return this;
        }

        public HtmlBuilder ul
        {
            get
            {
                result.Append("<ul>");
                tags.Push("ul");
                return this;
            }
        }

        public HtmlBuilder ol
        {
            get
            {
                result.Append("<ol>");
                tags.Push("ol");
                return this;
            }
        }

        public HtmlBuilder li
        {
            get
            {
                result.Append("<li>");
                tags.Push("li");
                return this;
            }
        }

        public HtmlBuilder end
        {
            get
            {
                string s = tags.Pop();
                result.Append("</").Append(s).Append(">");
                return this;
            }
        }

        public HtmlBuilder enter
        {
            get
            {
                result.Append("\n");
                return this;
            }
        }

        public HtmlBuilder include(HtmlBuilder hb)
        {
            result.Append(hb.ToString());
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
