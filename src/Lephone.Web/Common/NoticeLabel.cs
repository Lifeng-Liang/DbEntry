﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lephone.Web.Common
{
    public class NoticeLabel : Label
    {
        private bool _isInited;
        private LabelType _type;
        private bool _isModified;
        private List<string> _msgList;

        private enum LabelType
        {
            Notice,
            Warning,
            Tip,
        }

        protected override void OnLoad(System.EventArgs e)
        {
            Reset();
            base.OnLoad(e);
        }

        public void Reset()
        {
            Text = "";
            _isInited = false;
            _msgList = new List<string>();
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

        [Themeable(false), DefaultValue("Tip")]
        public string CssTip
        {
            get
            {
                object o = ViewState["CssTip"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Tip";
            }
            set
            {
                ViewState["CssTip"] = value;
            }
        }

        [Themeable(false), DefaultValue(false)]
        public bool SingleLine
        {
            get
            {
                object o = ViewState["SingleLine"];
                if (o != null)
                {
                    return (bool)o;
                }
                return false;
            }
            set
            {
                ViewState["SingleLine"] = value;
            }
        }

        public void AddWarning(string text)
        {
            if(SingleLine) { Reset(); }
            CheckType(LabelType.Warning);
            CssClass = CssWarning;
            _msgList.Add(text);
        }

        public void AddNotice(string text)
        {
            if (SingleLine) { Reset(); }
            CheckType(LabelType.Notice);
            CssClass = CssNotice;
            _msgList.Add(text);
        }

        public void AddTip(string text)
        {
            if (SingleLine) { Reset(); }
            CheckType(LabelType.Tip);
            CssClass = CssTip;
            _msgList.Add(text);
        }

        private void CheckType(LabelType lt)
        {
            if (_isInited)
            {
                if (_type != lt)
                {
                    throw new WebException("You add other type text before. One process only support one type of text.");
                }
            }
            else
            {
                _isInited = true;
                _type = lt;
            }
            _isModified = true;
            Visible = true;
        }

        private string GenerateText()
        {
            HtmlBuilder b = HtmlBuilder.New;
            if(SingleLine)
            {
                if(_msgList.Count > 0)
                {
                    b.text(_msgList[0]).enter();
                }
            }
            else
            {
                b.ul.enter();
                foreach (var s in _msgList)
                {
                    b.li.text(s).end.enter();
                }
                b.end.enter();
            }
            return b.ToString();
        }

        private void PreShowText()
        {
            if (!_isInited) return;
            if (!_isModified) return;
            Text = GenerateText();
            _isModified = false;
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

        public int Count
        {
            get
            {
                return _msgList.Count;
            }
        }
    }
}
