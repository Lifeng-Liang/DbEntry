
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

namespace Lephone.Web
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DbEntryDataBinder : Label
    {
        private Button _SaveButton;
        private IExcuteableDataSource _ds;
        private Label _PageTitle;

        public DbEntryDataBinder() { }

        [IDReferenceProperty(typeof(Button)), TypeConverter(typeof(ButtonIDConverter)), Themeable(false), DefaultValue("")]
        public string SaveButton
        {
            get
            {
                object o = this.ViewState["SaveButton"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["SaveButton"] = value;
            }
        }

        void _SaveButton_Click(object sender, EventArgs e)
        {
            object o = _ds.GetObject();
            object oid = ViewState["Id"];
            ValidateHandler vh = new ValidateHandler(true, false, InvalidFieldText, NotAllowNullText, NotMatchedText, LengthText, ShouldBeUniqueText);
            if (oid == null)
            {
                _ds.ValidateSave(vh, o, this, string.Format(ObjectCreatedText, _ds.GetClassName()));
            }
            else // Edit
            {
                _ds.SetKey(o, oid);
                _ds.ValidateSave(vh, o, this, string.Format(ObjectUpdatedText, _ds.GetClassName()));
            }
            this.Visible = true;
        }

        [IDReferenceProperty(typeof(DataSourceControl)), TypeConverter(typeof(ExcuteableDataSourceIDCoverter)), Themeable(false), DefaultValue("")]
        public string DataSourceID
        {
            get
            {
                object o = this.ViewState["DataSourceID"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["DataSourceID"] = value;
            }
        }

        [IDReferenceProperty(typeof(Label)), TypeConverter(typeof(LabelIDConverter)), Themeable(false), DefaultValue("")]
        public string PageTitle
        {
            get
            {
                object o = this.ViewState["PageTitle"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["PageTitle"] = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Visible = false;
            _SaveButton = this.NamingContainer.FindControl(SaveButton) as Button;
            if (_SaveButton == null)
            {
                throw new WebException("SaveButton must set!");
            }
            _ds = this.NamingContainer.FindControl(DataSourceID) as IExcuteableDataSource;
            if (_ds == null)
            {
                throw new WebException("DataSourceID must set!");
            }
            _PageTitle = this.NamingContainer.FindControl(PageTitle) as Label;

            _SaveButton.Click += new EventHandler(_SaveButton_Click);

            if (!Page.IsPostBack)
            {
                string sid = Page.Request["Id"];
                if (!string.IsNullOrEmpty(sid))
                {
                    ViewState["Id"] = _ds.SetControls(sid);
                    if (_PageTitle != null)
                    {
                        _PageTitle.Text = string.Format(EditObjectText, _ds.GetClassName());
                    }
                }
                else
                {
                    if (_PageTitle != null)
                    {
                        _PageTitle.Text = string.Format(NewObjectText, _ds.GetClassName());
                    }
                }
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_SaveButton != null)
            {
                _SaveButton.Click -= new EventHandler(_SaveButton_Click);
            }
            base.OnUnload(e);
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

        [Themeable(false), DefaultValue("Not Allow Null, ")]
        public string NotAllowNullText
        {
            get
            {
                object o = this.ViewState["NotAllowNullText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Not Allow Null, ";
            }
            set
            {
                this.ViewState["NotAllowNullText"] = value;
            }
        }

        [Themeable(false), DefaultValue("Not Matched, ")]
        public string NotMatchedText
        {
            get
            {
                object o = this.ViewState["NotMatchedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Not Matched, ";
            }
            set
            {
                this.ViewState["NotMatchedText"] = value;
            }
        }

        [Themeable(false), DefaultValue("The length should be {0} to {1} but was {2}, ")]
        public string LengthText
        {
            get
            {
                object o = this.ViewState["LengthText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "The length should be {0} to {1} but was {2}, ";
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
    }
}
