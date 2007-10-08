
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

namespace Lephone.Web.Common
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DbEntryDataBinder : Label
    {
        private Button _SaveButton;
        private IExcuteableDataSource ds;

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
            object o = ds.GetObject();
            object oid = ViewState["Id"];
            if (oid == null)
            {
                ds.ValidateSave(o, this, ds.GetClassName() + " Created!");
            }
            else // Edit
            {
                ds.SetKey(o, oid);
                ds.ValidateSave(o, this, ds.GetClassName() + " Updated");
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Visible = false;
            _SaveButton = this.NamingContainer.FindControl(SaveButton) as Button;
            if (_SaveButton == null)
            {
                throw new WebException("SaveButton must set!");
            }
            ds = this.NamingContainer.FindControl(DataSourceID) as IExcuteableDataSource;
            if (ds == null)
            {
                throw new WebException("DataSourceID must set!");
            }

            _SaveButton.Click += new EventHandler(_SaveButton_Click);
            ds.SetControls(ViewState);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_SaveButton != null)
            {
                _SaveButton.Click -= new EventHandler(_SaveButton_Click);
            }
            base.OnUnload(e);
        }
    }
}
