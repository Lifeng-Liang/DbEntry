using Lephone.Data;

namespace Lephone.Web.Rails
{
    public class LinkTo
    {
        public class LinkToInfo
        {
            internal string TitleName;
            internal string ControllerName;
            internal string ActionName;
            internal string AddonInfo;
            internal object[] ParametersInfo;

            public LinkToInfo Action(string name)
            {
                this.ActionName = name;
                return this;
            }

            public LinkToInfo Title(string name)
            {
                this.TitleName = name;
                return this;
            }

            public LinkToInfo Addon(string addon)
            {
                this.AddonInfo = addon;
                return this;
            }

            public LinkToInfo Parameters(params object[] parameters)
            {
                this.ParametersInfo = parameters;
                return this;
            }

            public static implicit operator string(LinkToInfo info)
            {
                return info.ToString();
            }

            public UrlTo.UrlToInfo ToUrlToInfo()
            {
                var ut = new UrlTo.UrlToInfo
                {
                    ControllerName = ControllerName,
                    ActionName = ActionName,
                    ParametersInfo = ParametersInfo
                };
                return ut;
            }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(TitleName))
                {
                    throw new DataException("title can not be null or empty.");
                }
                string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                    ToUrlToInfo(),
                    TitleName,
                    AddonInfo == null ? "" : " " + AddonInfo);
                return ret;
            }
        }

        private readonly string _controllerName;

        public LinkTo(string controllerName)
        {
            this._controllerName = controllerName;
        }

        public LinkToInfo Controller(string name)
        {
            var result = new LinkToInfo { ControllerName = name };
            return result;
        }

        public LinkToInfo Action(string name)
        {
            var result = new LinkToInfo {ControllerName = this._controllerName, ActionName = name};
            return result;
        }

        public LinkToInfo Title(string name)
        {
            var result = new LinkToInfo {ControllerName = this._controllerName, TitleName = name};
            return result;
        }
    }
}
