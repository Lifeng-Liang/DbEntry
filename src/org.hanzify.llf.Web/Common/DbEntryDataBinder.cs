
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
    public class DbEntryDataBinder<T> : Label
    {
        private Button _SaveButton;

        [IDReferenceProperty, TypeConverter(typeof(ButtonIDConverter)), Themeable(false), DefaultValue("")]
        public string SaveButton
        {
            get
            {
                object obj2 = this.ViewState[typeof(T).Name + "_SaveButton"];
                if (obj2 != null)
                {
                    return (string)obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState[typeof(T).Name + "_SaveButton"] = value;
            }
        }

        void _SaveButton_Click(object sender, EventArgs e)
        {
            string tn = typeof(T).Name;
            object oid = ViewState[tn + "_Id"];
            if (oid == null)
            {
                T o = PageHelper.GetObject<T>(Page);
                PageHelper.ValidateSave(o, this, tn + " Created!");
            }
            else // Edit
            {
                T o = PageHelper.GetObject<T>(Page);
                ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
                oi.KeyFields[0].SetValue(o, oid);
                PageHelper.ValidateSave(o, this, tn + " Updated");
            }
            this.Visible = true;
        }

        public DbEntryDataBinder()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Visible = false;
            _SaveButton = this.NamingContainer.FindControl(SaveButton) as Button;
            if (_SaveButton != null)
            {
                _SaveButton.Click += new EventHandler(_SaveButton_Click);
            }
            else
            {
                throw new DbEntryException("SaveButton must set!");
            }
            if (!Page.IsPostBack)
            {
                string sid = Page.Request["Id"];
                if (!string.IsNullOrEmpty(sid))
                {
                    ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
                    object Id = Convert.ChangeType(sid, oi.KeyFields[0].FieldType);
                    T o = DbEntry.GetObject<T>(Id);
                    PageHelper.SetObject(o, Page);
                    ViewState[typeof(T).Name + "_Id"] = Id;
                }
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            _SaveButton.Click -= new EventHandler(_SaveButton_Click);
            base.OnUnload(e);
        }
    }
}
