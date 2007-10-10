
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Security.Permissions;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Web.Common;
using Lephone.Util;

namespace Lephone.Web
{
    public abstract partial class DbEntryDataSource<T>
    {
        public event CallbackVoidHandler OnPageIsNew;
        public event CallbackVoidHandler OnPageIsEdit;

        private Button SaveButton;
        private Label ContentTitle;
        private Label NoticeMessage;

        [IDReferenceProperty(typeof(Button)), TypeConverter(typeof(ButtonIDConverter)), Themeable(false), DefaultValue(""), Category("Behavior")]
        public string SaveButtonID
        {
            get
            {
                object o = this.ViewState["SaveButtonID"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["SaveButtonID"] = value;
            }
        }

        [IDReferenceProperty(typeof(Label)), TypeConverter(typeof(LabelIDConverter)), Themeable(false), DefaultValue(""), Category("Behavior")]
        public string ContentTitleID
        {
            get
            {
                object o = this.ViewState["ContentTitleID"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ContentTitleID"] = value;
            }
        }

        [IDReferenceProperty(typeof(Label)), TypeConverter(typeof(LabelIDConverter)), Themeable(false), DefaultValue(""), Category("Behavior")]
        public string NoticeMessageID
        {
            get
            {
                object o = this.ViewState["NoticeMessageID"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["NoticeMessageID"] = value;
            }
        }

        void SaveButton_Click(object sender, EventArgs e)
        {
            object o = PageHelper.GetObject<T>(Page);
            object oid = ViewState["Id"];
            ValidateHandler vh = new ValidateHandler(EmptyAsNull, IncludeClassName, InvalidFieldText,
                NotAllowNullText, NotMatchedText, LengthText, ShouldBeUniqueText, SeparatorText);

            string tn = typeof(T).Name;
            if (oid == null)
            {
                PageHelper.ValidateSave(vh, o, NoticeMessage, string.Format(ObjectCreatedText, tn), CssNotice, CssWarning);
            }
            else // Edit
            {
                ObjInfo.KeyFields[0].SetValue(o, oid);
                PageHelper.ValidateSave(vh, o, NoticeMessage, string.Format(ObjectUpdatedText, tn), CssNotice, CssWarning);
            }
            if (NoticeMessage != null)
            {
                NoticeMessage.Visible = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SaveButton = this.NamingContainer.FindControl(SaveButtonID) as Button;
            ContentTitle = this.NamingContainer.FindControl(ContentTitleID) as Label;
            NoticeMessage = this.NamingContainer.FindControl(NoticeMessageID) as Label;

            if (NoticeMessage != null)
            {
                NoticeMessage.Visible = false;
            }

            if (SaveButton != null)
            {
                SaveButton.Click += new EventHandler(SaveButton_Click);
                if (!Page.IsPostBack)
                {
                    string tn = typeof(T).Name;
                    string sid = Page.Request["Id"];
                    if (!string.IsNullOrEmpty(sid))
                    {
                        object Id = Convert.ChangeType(sid, ObjInfo.KeyFields[0].FieldType);
                        T o = DbEntry.GetObject<T>(Id);
                        PageHelper.SetObject(o, Page);
                        ViewState["Id"] = Id;
                        if (OnPageIsNew != null)
                        {
                            OnPageIsEdit();
                        }
                        if (ContentTitle != null)
                        {
                            ContentTitle.Text = string.Format(EditObjectText, tn);
                        }
                    }
                    else
                    {
                        if (OnPageIsNew != null)
                        {
                            OnPageIsNew();
                        }
                        if (ContentTitle != null)
                        {
                            ContentTitle.Text = string.Format(NewObjectText, tn);
                        }
                    }
                }
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (SaveButton != null)
            {
                SaveButton.Click -= new EventHandler(SaveButton_Click);
            }
            base.OnUnload(e);
        }

        [Themeable(false), DefaultValue(false)]
        public bool EmptyAsNull
        {
            get
            {
                object o = this.ViewState["EmptyAsNull"];
                if (o != null)
                {
                    return (bool)o;
                }
                return false;
            }
            set
            {
                this.ViewState["EmptyAsNull"] = value;
            }
        }

        [Themeable(false), DefaultValue(false)]
        public bool IncludeClassName
        {
            get
            {
                object o = this.ViewState["IncludeClassName"];
                if (o != null)
                {
                    return (bool)o;
                }
                return false;
            }
            set
            {
                this.ViewState["IncludeClassName"] = value;
            }
        }

        [Themeable(false), DefaultValue("{0} Created!")]
        public string ObjectCreatedText
        {
            get
            {
                object o = this.ViewState["ObjectCreatedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "{0} Created!";
            }
            set
            {
                this.ViewState["ObjectCreatedText"] = value;
            }
        }

        [Themeable(false), DefaultValue("{0} Updated!")]
        public string ObjectUpdatedText
        {
            get
            {
                object o = this.ViewState["ObjectUpdatedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "{0} Updated!";
            }
            set
            {
                this.ViewState["ObjectUpdatedText"] = value;
            }
        }

        [Themeable(false), DefaultValue("Invalid Field {0} {1}.")]
        public string InvalidFieldText
        {
            get
            {
                object o = this.ViewState["InvalidFieldText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Invalid Field {0} {1}.";
            }
            set
            {
                this.ViewState["InvalidFieldText"] = value;
            }
        }

        [Themeable(false), DefaultValue("Not Allow Null")]
        public string NotAllowNullText
        {
            get
            {
                object o = this.ViewState["NotAllowNullText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Not Allow Null";
            }
            set
            {
                this.ViewState["NotAllowNullText"] = value;
            }
        }

        [Themeable(false), DefaultValue("Not Matched")]
        public string NotMatchedText
        {
            get
            {
                object o = this.ViewState["NotMatchedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Not Matched";
            }
            set
            {
                this.ViewState["NotMatchedText"] = value;
            }
        }

        [Themeable(false), DefaultValue("The length should be {0} to {1} but was {2}")]
        public string LengthText
        {
            get
            {
                object o = this.ViewState["LengthText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "The length should be {0} to {1} but was {2}";
            }
            set
            {
                this.ViewState["LengthText"] = value;
            }
        }

        [Themeable(false), DefaultValue("Should be UNIQUE")]
        public string ShouldBeUniqueText
        {
            get
            {
                object o = this.ViewState["ShouldBeUniqueText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Should be UNIQUE";
            }
            set
            {
                this.ViewState["ShouldBeUniqueText"] = value;
            }
        }

        [Themeable(false), DefaultValue(", ")]
        public string SeparatorText
        {
            get
            {
                object o = this.ViewState["SeparatorText"];
                if (o != null)
                {
                    return (string)o;
                }
                return ", ";
            }
            set
            {
                this.ViewState["SeparatorText"] = value;
            }
        }

        [Themeable(false), DefaultValue("New {0}")]
        public string NewObjectText
        {
            get
            {
                object o = this.ViewState["NewObjectText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "New {0}";
            }
            set
            {
                this.ViewState["NewObjectText"] = value;
            }
        }

        [Themeable(false), DefaultValue("{0} Edit")]
        public string EditObjectText
        {
            get
            {
                object o = this.ViewState["EditObjectText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "{0} Edit";
            }
            set
            {
                this.ViewState["EditObjectText"] = value;
            }
        }

        [Themeable(false), DefaultValue("Notice")]
        public string CssNotice
        {
            get
            {
                object o = this.ViewState["CssNotice"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Notice";
            }
            set
            {
                this.ViewState["CssNotice"] = value;
            }
        }

        [Themeable(false), DefaultValue("Warning")]
        public string CssWarning
        {
            get
            {
                object o = this.ViewState["CssWarning"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Warning";
            }
            set
            {
                this.ViewState["CssWarning"] = value;
            }
        }
    }
}
