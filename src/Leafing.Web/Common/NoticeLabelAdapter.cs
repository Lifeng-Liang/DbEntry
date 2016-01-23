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
		private string cssBase;

		public NoticeLabelAdapter (Label label)
		{
			_innerLabel = label;
			Reset();
			cssBase = PageHelper.GetCssBase(label.CssClass);
		}

		private void Reset()
        {
			_innerLabel.Text = "";
            _msgList = new List<string>();
			_innerLabel.Visible = false;
        }

		public void AddMessage(string text)
		{
			_msgList.Add(text);
		}

		public void ShowWith(string cssClass)
		{
			if (_innerLabel != null) {
				_innerLabel.Text = GenerateText();
				_innerLabel.CssClass = cssBase + cssClass;
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
