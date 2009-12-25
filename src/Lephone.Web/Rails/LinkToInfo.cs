using Lephone.Data;

namespace Lephone.Web.Rails
{
    public class LinkToInfo : UrlToInfo
    {
        protected string TheTitle;
        protected string TheAddon;

        public LinkToInfo()
        {
        }

        public LinkToInfo(string controller)
        {
            this.TheController = controller;
        }

        public new LinkToInfo Controller(string name)
        {
            base.Controller(name);
            return this;
        }

        public new LinkToInfo Action(string name)
        {
            base.Action(name);
            return this;
        }

        public LinkToInfo Title(string name)
        {
            this.TheTitle = name;
            return this;
        }

        public LinkToInfo Addon(string addon)
        {
            this.TheAddon = addon;
            return this;
        }

        public new LinkToInfo Parameters(params object[] parameters)
        {
            base.Parameters(parameters);
            return this;
        }

        public new LinkToInfo UrlParam(string key, string value)
        {
            base.UrlParam(key, value);
            return this;
        }

        public static implicit operator string(LinkToInfo info)
        {
            return info.ToString();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(TheTitle))
            {
                throw new DataException("title can not be null or empty.");
            }
            string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                base.ToString(),
                TheTitle,
                TheAddon == null ? "" : " " + TheAddon);
            return ret;
        }
    }
}
