using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Leafing.Web.Common
{
    public class NoticeLabelAdapter
    {
		private Label _innerLabel;
        private List<string> _msgList;
		private string _cssBase;
		private string _cssNotice;
		private string _cssWarning;

		public NoticeLabelAdapter (Label label, string cssNotice, string cssWarning)
		{
			_innerLabel = label;
			_cssNotice = cssNotice;
			_cssWarning = cssWarning;
			_cssBase = PageHelper.GetOriginCss(label.CssClass, cssNotice);
			_cssBase = PageHelper.GetOriginCss(_cssBase, cssWarning);
			_cssBase = PageHelper.GetCssBase(_cssBase);
			Reset();
		}

		private void Reset()
        {
			_innerLabel.Text = "";
            _msgList = new List<string>();
			_innerLabel.Visible = false;
			_innerLabel.CssClass = _cssBase;
        }

		public void AddMessage(string text)
		{
			_msgList.Add(text);
		}

		public void ShowNotice()
		{
			ShowWith(_cssNotice);
		}

		public void ShowWarning()
		{
			ShowWith(_cssWarning);
		}

		public void ShowWith(string cssClass)
		{
			if (_innerLabel != null) {
				_innerLabel.Text = GenerateText();
				_innerLabel.CssClass = _cssBase + cssClass;
				_innerLabel.Visible = true;
			}
		}

        private string GenerateText()
        {
            var b = HtmlBuilder.New;
			b.ul.enter();
			foreach (var s in _msgList)
			{
				b.li.text(s).end.enter();
			}
			b.end.enter();
            return b.ToString();
        }
    }
}
