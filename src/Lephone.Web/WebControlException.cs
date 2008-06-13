using System.Web.UI.WebControls;

namespace Lephone.Web
{
    public class WebControlException : WebException
    {
        private readonly WebControl _RelatedControl;

        public WebControl RelatedControl
        {
            get { return _RelatedControl; }
        }

		public WebControlException(WebControl RelatedControl, string ErrorMessage) : base(ErrorMessage)
        {
            _RelatedControl = RelatedControl;
        }
    }
}
