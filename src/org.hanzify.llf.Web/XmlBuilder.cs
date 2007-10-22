using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Lephone.Web
{
    public abstract class XmlBuilder<T> where T : XmlBuilder<T>
    {
        protected StringBuilder result = new StringBuilder();
        protected Stack<string> tags = new Stack<string>();
        protected string ctag = string.Empty;

        public T text(string text)
        {
            // Is there XmlUtility.XmlEncode ?
            return include(HttpUtility.HtmlEncode(text));
        }

        public T text(object obj)
        {
            return text(obj.ToString());
        }

        public T end
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
                return (T)this;
            }
        }

        public T enter
        {
            get { return include("\r\n"); }
        }

        public T newline
        {
            get { return include("\n"); }
        }

        public T tab
        {
            get { return include("\t"); }
        }

        public T include(T hb)
        {
            return include(hb.ToString());
        }

        public T include(string text)
        {
            result.Append(text);
            ctag = string.Empty;
            return (T)this;
        }

        public T tag(string TagName)
        {
            result.Append("<").Append(TagName).Append(">");
            tags.Push(TagName);
            ctag = TagName;
            return (T)this;
        }

        public T attr(string Name, object Value)
        {
            return attr(Name, Value.ToString());
        }

        public T attr(string Name, string Value)
        {
            if (result.Length > 0 && result[result.Length - 1] == '>')
            {
                result.Length--;
                result.Append(" ").Append(Name).Append("=\"").Append(Value).Append("\">");
                return (T)this;
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
        public T over()
        {
            return (T)this;
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

    public class XmlBuilder : XmlBuilder<XmlBuilder>
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
    }
}
