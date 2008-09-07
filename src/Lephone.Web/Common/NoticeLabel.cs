using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lephone.Web.Common
{
    public class NoticeLabel : Label
    {
        private bool isInited;
        private bool isWarning;
        private bool isModified;
        private List<string> msgList;

        protected override void OnLoad(System.EventArgs e)
        {
            isInited = false;
            msgList = new List<string>();
            base.OnLoad(e);
            Visible = false;
        }

        [Themeable(false), DefaultValue("Warning")]
        public string CssWarning
        {
            get
            {
                object o = ViewState["CssWarning"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Warning";
            }
            set
            {
                ViewState["CssWarning"] = value;
            }
        }

        [Themeable(false), DefaultValue("Notice")]
        public string CssNotice
        {
            get
            {
                object o = ViewState["CssNotice"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Notice";
            }
            set
            {
                ViewState["CssNotice"] = value;
            }
        }

        public void AddWarning(string text)
        {
            if (isInited)
            {
                if (!isWarning)
                {
                    throw new WebException("You add notice before. One process only support one type of text.");
                }
            }
            else
            {
                isInited = true;
                isWarning = true;
            }
            isModified = true;
            Visible = true;
            CssClass = CssWarning;
            msgList.Add(text);
        }

        public void AddNotice(string text)
        {
            if (isInited)
            {
                if(isWarning)
                {
                    throw new WebException("You add warning before. One process only support one type of text.");
                }
            }
            else
            {
                isInited = true;
                isWarning = false;
            }
            isModified = true;
            Visible = true;
            CssClass = CssNotice;
            msgList.Add(text);
        }

        private string GenerateText()
        {
            HtmlBuilder b = HtmlBuilder.New.ul.enter();
            foreach (var s in msgList)
            {
                b.li.text(s).end.enter();
            }
            b.end.enter();
            return b.ToString();
        }

        private void PreShowText()
        {
            if (!isInited) return;
            if (!isModified) return;
            Text = GenerateText();
            isModified = false;
        }

        public override string Text
        {
            get
            {
                PreShowText();
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
    }
}
