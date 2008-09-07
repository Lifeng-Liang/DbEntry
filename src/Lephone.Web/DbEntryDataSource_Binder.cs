using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Web.Common;
using Lephone.Util;
using Lephone.Data.Definition;

namespace Lephone.Web
{
    public abstract partial class DbEntryDataSource<T> where T : class, IDbObject
    {
        public event CallbackVoidHandler OnPageIsNew;
        public event CallbackVoidHandler OnPageIsEdit;

        [Category("Data")]
        public event CallbackVoidHandler OnObjectLoading;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnObjectLoaded;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnValidateSave;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnObjectDeleting;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnObjectDeleted;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnObjectInserting;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnObjectInserted;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnObjectUpdating;

        [Category("Data")]
        public event CallbackObjectHandler<T> OnObjectUpdated;

        private Button SaveButton;
        private Button DeleteButton;
        private Label ContentTitle;
        private NoticeLabel NoticeMessage;

        private bool _LastOprationSucceed;

        public bool LastOprationSucceed
        {
            get { return _LastOprationSucceed; }
        }

        [IDReferenceProperty(typeof(Button)), TypeConverter(typeof(ButtonIDConverter)), Themeable(false),
         DefaultValue(""), Category("Behavior")]
        public string SaveButtonID
        {
            get
            {
                object o = ViewState["SaveButtonID"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set { ViewState["SaveButtonID"] = value; }
        }

        [IDReferenceProperty(typeof(Button)), TypeConverter(typeof(ButtonIDConverter)), Themeable(false),
         DefaultValue(""), Category("Behavior")]
        public string DeleteButtonID
        {
            get
            {
                object o = ViewState["DeleteButtonID"];
                if (o != null)
                {
                    return (string)o;
                }
                return "";
            }
            set { ViewState["DeleteButtonID"] = value; }
        }

        [IDReferenceProperty(typeof(Label)), TypeConverter(typeof(LabelIDConverter)), Themeable(false),
         DefaultValue(""), Category("Behavior")]
        public string ContentTitleID
        {
            get
            {
                object o = ViewState["ContentTitleID"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set { ViewState["ContentTitleID"] = value; }
        }

        [Themeable(false), DefaultValue(true), Category("Behavior")]
        public bool ChangePageTitleToo
        {
            get
            {
                object o = ViewState["ChangePageTitleToo "];
                if (o != null)
                {
                    return (bool)o;
                }
                return true;
            }
            set { ViewState["ChangePageTitleToo "] = value; }
        }

        [IDReferenceProperty(typeof(Label)), TypeConverter(typeof(NoticeLabelIDConverter)), Themeable(false),
         DefaultValue(""), Category("Behavior")]
        public string NoticeMessageID
        {
            get
            {
                object o = ViewState["NoticeMessageID"];
                if (o != null)
                {
                    return (string)o;
                }
                return string.Empty;
            }
            set { ViewState["NoticeMessageID"] = value; }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveObject();
        }

        public T SaveObject()
        {
            _LastOprationSucceed = false;
            try
            {
                if (OnObjectLoading != null)
                {
                    OnObjectLoading();
                }

                var o = PageHelper.GetObject<T>(Page, ParseErrorText);
                object oid = ViewState["Id"];

                string tn = typeof(T).Name;
                if (oid == null)
                {
                    if (OnObjectInserting != null)
                    {
                        OnObjectInserting(o);
                    }
                    if (ValidateSave(o, string.Format(ObjectCreatedText, tn)))
                    {
                        if (OnObjectInserted != null)
                        {
                            OnObjectInserted(o);
                        }
                        _LastOprationSucceed = true;
                        return o;
                    }
                }
                else // Edit
                {
                    ObjInfo.KeyFields[0].SetValue(o, oid);
                    if (OnObjectUpdating != null)
                    {
                        OnObjectUpdating(o);
                    }
                    if (ValidateSave(o, string.Format(ObjectUpdatedText, tn)))
                    {
                        if (OnObjectUpdated != null)
                        {
                            OnObjectUpdated(o);
                        }
                        _LastOprationSucceed = true;
                        return o;
                    }
                }
            }
            catch (WebControlException ex)
            {
                AddWarning(ex.RelatedControl, ex.Message);
            }
            catch (Exception ex)
            {
                AddWarning(ex.Message);
            }
            return default(T);
        }

        public void AddNotice(string msg)
        {
            if (NoticeMessage != null)
            {
                NoticeMessage.AddNotice(msg);
            }
        }

        public void AddWarning(string msg)
        {
            AddWarning(null, msg);
        }

        public void AddWarning(WebControl c, string msg)
        {
            if (NoticeMessage != null)
            {
                if (c != null)
                {
                    c.CssClass = CssErrInput;
                }
                NoticeMessage.AddWarning(msg);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            _LastOprationSucceed = false;
            try
            {
                object oid = ViewState["Id"];

                string tn = typeof(T).Name;
                if (oid != null)
                {
                    var o = DbEntry.GetObject<T>(oid);

                    if (OnObjectDeleting != null)
                    {
                        OnObjectDeleting(o);
                    }

                    ExecuteDelete(o);
                    AddNotice(string.Format(ObjectDeletedText, tn));

                    if (OnObjectDeleted != null)
                    {
                        OnObjectDeleted(o);
                    }
                    _LastOprationSucceed = true;
                }
            }
            catch (WebControlException ex)
            {
                AddWarning(ex.RelatedControl, ex.Message);
            }
            catch (Exception ex)
            {
                AddWarning(ex.Message);
            }
        }

        protected virtual bool ValidateSave(T obj, string NoticeText)
        {
            if (OnValidateSave != null)
            {
                OnValidateSave(obj);
            }

            var vh = new ValidateHandler(EmptyAsNull, IncludeClassName, InvalidFieldText,
                                                     NotAllowNullText, NotMatchedText, LengthText, ShouldBeUniqueText,
                                                     SeparatorText);

            return PageHelper.ValidateSave(Page, vh, obj, NoticeMessage, NoticeText, CssErrInput,
                delegate
                   {
                       ObjectInfo oi =
                           ObjectInfo.GetInstance(
                               obj.GetType());
                       if (oi.IsNewObject(obj))
                       {
                           ExecuteInsert(obj);
                       }
                       else
                       {
                           ExecuteUpdate(obj);
                       }
                   });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SaveButton = NamingContainer.FindControl(SaveButtonID) as Button;
            DeleteButton = NamingContainer.FindControl(DeleteButtonID) as Button;
            ContentTitle = NamingContainer.FindControl(ContentTitleID) as Label;
            NoticeMessage = NamingContainer.FindControl(NoticeMessageID) as NoticeLabel;

            if (SaveButton != null)
            {
                SaveButton.Click += SaveButton_Click;
                if (!Page.IsPostBack)
                {
                    string tn = typeof(T).Name;
                    T o = GetRequestObject();
                    if (o == null)
                    {
                        if (DeleteButton != null)
                        {
                            DeleteButton.Visible = false;
                        }
                        if (OnPageIsNew != null)
                        {
                            OnPageIsNew();
                        }
                        SetContentTitle(NewObjectText, tn);
                    }
                    else
                    {
                        PageHelper.SetObject(o, Page);
                        if (OnPageIsEdit != null)
                        {
                            OnPageIsEdit();
                        }
                        SetContentTitle(EditObjectText, tn);
                        if (OnObjectLoaded != null)
                        {
                            OnObjectLoaded(o);
                        }
                    }
                }
            }
            if (DeleteButton != null)
            {
                DeleteButton.Click += DeleteButton_Click;
                //TODO: why left like: T o = GetRequestObject();
                GetRequestObject();
            }
        }

        private void SetContentTitle(string textTemplate, string tn)
        {
            if (ContentTitle != null)
            {
                ContentTitle.Text = string.Format(textTemplate, tn);
                if (ChangePageTitleToo)
                {
                    Page.Title = ContentTitle.Text;
                }
            }
        }

        public void RebindControls()
        {
            T o = GetRequestObject();
            if (o != null)
            {
                PageHelper.SetObject(o, Page);
            }
        }

        protected T GetRequestObject()
        {
            string sid = Page.Request["Id"];
            if (!string.IsNullOrEmpty(sid))
            {
                object Id = ClassHelper.ChangeType(sid, ObjInfo.KeyFields[0].FieldType);
                var o = DbEntry.GetObject<T>(Id);
                if(o == null)
                {
                    throw new DataException("The record doesn't exist.");
                }
                ViewState["Id"] = Id;
                ObjectInfo oi = ObjectInfo.GetInstance(typeof (T));
                if(oi.LockVersion != null)
                {
                    var lv = (int)oi.LockVersion.GetValue(o);
                    if (Page.IsPostBack)
                    {
                        if(lv != ObjectLockVersion)
                        {
                            throw new DataException("The version of record was changed.");
                        }
                    }
                    else
                    {
                        ObjectLockVersion = lv;
                    }
                }
                return o;
            }
            return default(T);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (SaveButton != null)
            {
                SaveButton.Click -= SaveButton_Click;
            }
            if (DeleteButton != null)
            {
                DeleteButton.Click -= DeleteButton_Click;
            }
            base.OnUnload(e);
        }

        [Themeable(false), DefaultValue(false)]
        public bool EmptyAsNull
        {
            get
            {
                object o = ViewState["EmptyAsNull"];
                if (o != null)
                {
                    return (bool)o;
                }
                return false;
            }
            set { ViewState["EmptyAsNull"] = value; }
        }

        [Themeable(false), DefaultValue(false)]
        public bool IncludeClassName
        {
            get
            {
                object o = ViewState["IncludeClassName"];
                if (o != null)
                {
                    return (bool)o;
                }
                return false;
            }
            set { ViewState["IncludeClassName"] = value; }
        }

        [Themeable(false), DefaultValue("{0} Created!")]
        public string ObjectCreatedText
        {
            get
            {
                object o = ViewState["ObjectCreatedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "{0} Created!";
            }
            set { ViewState["ObjectCreatedText"] = value; }
        }

        [Themeable(false), DefaultValue("{0} Updated!")]
        public string ObjectUpdatedText
        {
            get
            {
                object o = ViewState["ObjectUpdatedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "{0} Updated!";
            }
            set { ViewState["ObjectUpdatedText"] = value; }
        }

        [Themeable(false), DefaultValue("{0} Deleted!")]
        public string ObjectDeletedText
        {
            get
            {
                object o = ViewState["ObjectDeletedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "{0} Deleted!";
            }
            set { ViewState["ObjectDeletedText"] = value; }
        }

        [Themeable(false), DefaultValue("Invalid Field {0} {1}.")]
        public string InvalidFieldText
        {
            get
            {
                object o = ViewState["InvalidFieldText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Invalid Field {0} {1}.";
            }
            set { ViewState["InvalidFieldText"] = value; }
        }

        [Themeable(false), DefaultValue("Not Allow Null")]
        public string NotAllowNullText
        {
            get
            {
                object o = ViewState["NotAllowNullText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Not Allow Null";
            }
            set { ViewState["NotAllowNullText"] = value; }
        }

        [Themeable(false), DefaultValue("Not Matched")]
        public string NotMatchedText
        {
            get
            {
                object o = ViewState["NotMatchedText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Not Matched";
            }
            set { ViewState["NotMatchedText"] = value; }
        }

        [Themeable(false), DefaultValue("The length should be {0} to {1} but was {2}")]
        public string LengthText
        {
            get
            {
                object o = ViewState["LengthText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "The length should be {0} to {1} but was {2}";
            }
            set { ViewState["LengthText"] = value; }
        }

        [Themeable(false), DefaultValue("Should be UNIQUED")]
        public string ShouldBeUniqueText
        {
            get
            {
                object o = ViewState["ShouldBeUniqueText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Should be UNIQUED";
            }
            set { ViewState["ShouldBeUniqueText"] = value; }
        }

        [Themeable(false), DefaultValue(", ")]
        public string SeparatorText
        {
            get
            {
                object o = ViewState["SeparatorText"];
                if (o != null)
                {
                    return (string)o;
                }
                return ", ";
            }
            set { ViewState["SeparatorText"] = value; }
        }

        [Themeable(false), DefaultValue("New {0}")]
        public string NewObjectText
        {
            get
            {
                object o = ViewState["NewObjectText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "New {0}";
            }
            set { ViewState["NewObjectText"] = value; }
        }

        [Themeable(false), DefaultValue("{0} Edit")]
        public string EditObjectText
        {
            get
            {
                object o = ViewState["EditObjectText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "{0} Edit";
            }
            set { ViewState["EditObjectText"] = value; }
        }

        [Themeable(false), DefaultValue("ErrInput")]
        public string CssErrInput
        {
            get
            {
                object o = ViewState["CssErrInput"];
                if (o != null)
                {
                    return (string)o;
                }
                return "ErrInput";
            }
            set { ViewState["CssErrInput"] = value; }
        }

        [Themeable(false), DefaultValue("Field [{0}] parse error: {1}")]
        public string ParseErrorText
        {
            get
            {
                object o = ViewState["ParseErrorText"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Field [{0}] parse error: {1}";
            }
            set { ViewState["ParseErrorText"] = value; }
        }

        private int ObjectLockVersion
        {
            get
            {
                object o = ViewState["ObjectLockVersion"];
                if (o != null)
                {
                    return (int)o;
                }
                return -1;
            }
            set
            {
                ViewState["ObjectLockVersion"] = value;
            }
        }
    }
}