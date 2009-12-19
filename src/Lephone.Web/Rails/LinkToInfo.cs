using Lephone.Data;

namespace Lephone.Web.Rails
{
    public class LinkToInfo : UrlToInfo
    {
        public class LinkTo
        {
            private readonly string _controllerName;

            public LinkTo(string controllerName)
            {
                this._controllerName = controllerName;
            }

            public LinkToInfo Controller(string name)
            {
                var result = new LinkToInfo { _controller = name };
                return result;
            }

            public LinkToInfo Action(string name)
            {
                var result = new LinkToInfo { _controller = this._controllerName, _action = name };
                return result;
            }

            public LinkToInfo Title(string name)
            {
                var result = new LinkToInfo { _controller = this._controllerName, _title = name };
                return result;
            }
        }

        protected string _title;
        protected string _addon;

        public LinkToInfo()
        {
        }

        public LinkToInfo(string controller)
        {
            this._controller = controller;
        }

        public new LinkToInfo Action(string name)
        {
            base.Action(name);
            return this;
        }

        public LinkToInfo Title(string name)
        {
            this._title = name;
            return this;
        }

        public LinkToInfo Addon(string addon)
        {
            this._addon = addon;
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
            if (string.IsNullOrEmpty(_title))
            {
                throw new DataException("title can not be null or empty.");
            }
            string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                base.ToString(),
                _title,
                _addon == null ? "" : " " + _addon);
            return ret;
        }
    }
}
