using Lephone.Data;

namespace Lephone.Web.Rails
{
    public class LinkToInfo
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

        string _title;
        string _controller;
        string _action;
        string _addon;
        object[] _parameters;

        public LinkToInfo()
        {
        }

        public LinkToInfo(string controller)
        {
            this._controller = controller;
        }

        public LinkToInfo Action(string name)
        {
            this._action = name;
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

        public LinkToInfo Parameters(params object[] parameters)
        {
            this._parameters = parameters;
            return this;
        }

        public static implicit operator string(LinkToInfo info)
        {
            return info.ToString();
        }

        public UrlToInfo ToUrlToInfo()
        {
            var ut = new UrlToInfo(_controller).Action(_action).Parameters(_parameters);
            return ut;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_title))
            {
                throw new DataException("title can not be null or empty.");
            }
            string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                ToUrlToInfo(),
                _title,
                _addon == null ? "" : " " + _addon);
            return ret;
        }
    }
}
