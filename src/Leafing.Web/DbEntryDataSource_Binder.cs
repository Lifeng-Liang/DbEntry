using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Leafing.Data;
using Leafing.Web.Common;
using Leafing.Core;
using Leafing.Data.Definition;

namespace Leafing.Web
{
    public abstract partial class DbEntryDataSource<T> where T : class, IDbObject, new()
    {
        public event Action PageIsNew;
        public event Action PageIsEdit;

        [Category("Data")]
        public event Action ObjectLoading;

        [Category("Data")]
        public event Action<T> ObjectLoaded;

        [Category("Data")]
        public event Action<T> ValidateSaving;

        [Category("Data")]
        public event Action<T> ObjectDeleting;

        [Category("Data")]
        public event Action<T> ObjectDeleted;

        [Category("Data")]
        public event Action<T> ObjectInserting;

        [Category("Data")]
        public event Action<T> ObjectInserted;

        [Category("Data")]
        public event Action<T> ObjectUpdating;

        [Category("Data")]
        public event Action<T> ObjectUpdated;

        private Button _saveButton;
        private Button _deleteButton;
        private Label _contentTitle;
        private NoticeLabel _noticeMessage;

        private bool _lastOprationSucceed;

        public bool LastOprationSucceed
        {
            get { return _lastOprationSucceed; }
        }

        private void RaiseEvent(Action<T> evt, T obj)
        {
            if(evt != null)
            {
                evt(obj);
            }
        }

        private void RaiseEvent(Action evt)
        {
            if(evt != null)
            {
                evt();
            }
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
            _lastOprationSucceed = false;
            try
            {
                object oid = ViewState["Id"];

                string tn = typeof(T).Name;
                if (oid == null)
                {
                    var o = PageHelper.GetObject<T>(Page, ParseErrorText);
                    if (ValidateSave(o, string.Format(ObjectCreatedText, tn)))
                    {
                        _lastOprationSucceed = true;
                        return o;
                    }
                }
                else // Edit
                {
                    var o = PageHelper.GetObject<T>(oid, Page, ParseErrorText);
                    //Ctx.Info.KeyMembers[0].SetValue(o, oid);
                    if (ValidateSave(o, string.Format(ObjectUpdatedText, tn)))
                    {
                        _lastOprationSucceed = true;
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
            if (_noticeMessage != null)
            {
                _noticeMessage.AddNotice(msg);
            }
        }

        public void AddWarning(string msg)
        {
            AddWarning(null, msg);
        }

        public void AddWarning(WebControl c, string msg)
        {
            if (_noticeMessage != null)
            {
                if (c != null)
                {
                    c.CssClass = CssErrInput;
                }
                _noticeMessage.AddWarning(msg);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            _lastOprationSucceed = false;
            try
            {
                object oid = ViewState["Id"];

                string tn = typeof(T).Name;
                if (oid != null)
                {
                    var o = DbEntry.GetObject<T>(oid);
                    ExecuteDelete(o);
                    AddNotice(string.Format(ObjectDeletedText, tn));
                    _lastOprationSucceed = true;
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

        protected virtual bool ValidateSave(T obj, string noticeText)
        {
            RaiseEvent(ValidateSaving, obj);

            var vh = new ValidateHandler(EmptyAsNull, IncludeClassName, InvalidFieldText,
                                                     NotAllowNullText, NotMatchedText, LengthText, ShouldBeUniqueText,
                                                     SeparatorText);

            return PageHelper.ValidateSave(Page, vh, obj, _noticeMessage, noticeText, CssErrInput,
                delegate
                   {
                       var ctx = ModelContext.GetInstance(obj.GetType());
                       if (ctx.IsNewObject(obj))
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

            try
            {
                _saveButton = NamingContainer.FindControl(SaveButtonID) as Button;
                _deleteButton = NamingContainer.FindControl(DeleteButtonID) as Button;
                _contentTitle = NamingContainer.FindControl(ContentTitleID) as Label;
                _noticeMessage = NamingContainer.FindControl(NoticeMessageID) as NoticeLabel;

                if (_saveButton != null)
                {
                    InitSaveButton();
                }
                if (_deleteButton != null)
                {
                    _deleteButton.Click += DeleteButton_Click;
                    GetRequestObject();
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

        private void InitSaveButton()
        {
            _saveButton.Click += SaveButton_Click;
            if (!Page.IsPostBack)
            {
                string tn = typeof(T).Name;
                T o = GetRequestObject();
                if (o == null)
                {
                    if (_deleteButton != null)
                    {
                        _deleteButton.Visible = false;
                    }
                    RaiseEvent(PageIsNew);
                    SetContentTitle(NewObjectText, tn);
                }
                else
                {
                    RaiseEvent(ObjectLoading);
                    PageHelper.SetObject(o, Page);
                    RaiseEvent(PageIsEdit);
                    SetContentTitle(EditObjectText, tn);
                    RaiseEvent(ObjectLoaded, o);
                }
            }
        }

        private void SetContentTitle(string textTemplate, string tn)
        {
            if (_contentTitle != null)
            {
                _contentTitle.Text = string.Format(textTemplate, tn);
                if (ChangePageTitleToo)
                {
                    Page.Title = _contentTitle.Text;
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
                object id = ClassHelper.ChangeType(sid, Ctx.Info.KeyMembers[0].MemberType);
                var o = DbEntry.GetObject<T>(id);
                if(o == null)
                {
                    throw new DataException("The record doesn't exist.");
                }
                ViewState["Id"] = id;
                var ctx = ModelContext.GetInstance(typeof(T));
                if(ctx.Info.LockVersion != null)
                {
                    var lv = (int)ctx.Info.LockVersion.GetValue(o);
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
            if (_saveButton != null)
            {
                _saveButton.Click -= SaveButton_Click;
            }
            if (_deleteButton != null)
            {
                _deleteButton.Click -= DeleteButton_Click;
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