using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Lephone.Web
{
    public class XmlBuilder
    {
        public static XmlBuilder New()
        {
            return new XmlBuilder();
        }

        public static XmlBuilder New(string Version, string Encoding)
        {
            if (string.IsNullOrEmpty(Version))
                Version = "1.0";
            if (string.IsNullOrEmpty(Encoding))
                Encoding = "utf-8";
            return new XmlBuilder(Version, Encoding);
        }

        public XmlBuilder()
        {
        }

        public XmlBuilder(string Version, string Encoding)
        {
            result.Append("<?xml version=\"").Append(Version).Append("\" encoding=\"").Append(Encoding).Append("\" ?>\r\n");
        }

        protected StringBuilder result = new StringBuilder();
        protected Stack<string> tags = new Stack<string>();
        protected string ctag = string.Empty;

        public XmlBuilder text(string text)
        {
            // Is there XmlUtility.XmlEncode ?
            return include(HttpUtility.HtmlEncode(text));
        }

        public XmlBuilder text(object obj)
        {
            return text(obj.ToString());
        }

        public XmlBuilder end
        {
            get
            {
                string s = tags.Pop();
                if (s == ctag && result[result.Length - 1] == '>')
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

        public XmlBuilder enter
        {
            get { return include("\r\n"); }
        }

        public XmlBuilder newline
        {
            get { return include("\n"); }
        }

        public XmlBuilder tab
        {
            get { return include("\t"); }
        }

        public XmlBuilder include(XmlBuilder hb)
        {
            return include(hb.ToString());
        }

        public XmlBuilder include(string text)
        {
            result.Append(text);
            ctag = string.Empty;
            return this;
        }

        public XmlBuilder tag(string TagName)
        {
            result.Append("<").Append(TagName).Append(">");
            tags.Push(TagName);
            ctag = TagName;
            return this;
        }

        public XmlBuilder attr(string Name, object Value)
        {
            return attr(Name, Value.ToString());
        }

        public XmlBuilder attr(string Name, string Value)
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
        public XmlBuilder over()
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
