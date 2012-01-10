using System.Text;
using Leafing.Web.Mvc.Core;

namespace Leafing.Web.Mvc
{
    public class TextView : IView
    {
        protected StringBuilder Builder = new StringBuilder();

        public TextView()
        {
        }

        public TextView(string s)
        {
            Append(s);
        }

        public TextView(string template, params object[] args)
        {
            Append(string.Format(template, args));
        }

        public TextView Append(string s)
        {
            Builder.Append(s);
            return this;
        }

        public int Length
        {
            get { return Builder.Length; }
            set { Builder.Length = value; }
        }

        void IView.Render()
        {
            HttpContextHandler.Instance.Write(Builder.ToString());
        }
    }
}
