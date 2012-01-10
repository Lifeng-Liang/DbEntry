using System;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;

namespace Leafing.Web
{
    [Serializable]
    public class WebControlException : WebException
    {
        private readonly WebControl _relatedControl;

        public WebControl RelatedControl
        {
            get { return _relatedControl; }
        }

		public WebControlException(WebControl relatedControl, string errorMessage) : base(errorMessage)
        {
            _relatedControl = relatedControl;
        }

        protected WebControlException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
