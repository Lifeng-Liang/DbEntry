using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Lephone.Web
{
    public class WebControlException : WebException
    {
        private WebControl _RelatedControl;

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
