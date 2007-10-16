
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lephone.Util;
using Lephone.Data;
using Lephone.Data.Common;

namespace Lephone.Web.Common
{
    public static class PageHelper
    {
        public static void ValidateSave(object obj, Label msg, string NoticeText)
        {
            ValidateHandler vh = new ValidateHandler();
            ValidateSave(vh, obj, msg, NoticeText, "Notice", "Warning");
        }
        
        public static void ValidateSave(ValidateHandler vh, object obj, Label msg, string NoticeText, string CssNotice, string CssWarning)
        {
            ValidateSave(vh, obj, msg, NoticeText, CssNotice, CssWarning, delegate()
            {
                DbEntry.Save(obj);
            });
        }

        public static void ValidateSave(ValidateHandler vh, object obj, Label msg, string NoticeText,
            string CssNotice, string CssWarning, CallbackVoidHandler callback)
        {
            vh.ValidateObject(obj);
            if (vh.IsValid)
            {
                callback();
                if (msg != null)
                {
                    msg.CssClass = CssNotice;
                    msg.Text = HttpUtility.HtmlEncode(NoticeText);
                    msg.Visible = true;
                }
            }
            else
            {
                HtmlBuilder b = HtmlBuilder.New.ul;
                foreach (string str in vh.ErrorMessages.Keys)
                {
                    b = b.li.text(vh.ErrorMessages[str]).end;
                }
                b = b.end;
                if (msg != null)
                {
                    msg.CssClass = CssWarning;
                    msg.Text = b.ToString();
                    msg.Visible = true;
                }
            }
        }

        private static void EnumControls(Page p, ObjectInfo oi, CallbackObjectHandler2<MemberHandler, Control> callback)
        {
            foreach (MemberHandler h in oi.SimpleFields)
            {
                if (!h.IsKey)
                {
                    string cid = string.Format("{0}_{1}", oi.BaseType.Name, h.MemberInfo.Name);
                    Control c = ClassHelper.GetValue(p, cid) as Control;
                    if (c != null)
                    {
                        callback(h, c);
                    }
                    else
                    {
                        if (!(h.IsCreatedOn || h.IsUpdatedOn))
                        {
                            throw new DataException(string.Format("Control {0} not find!", cid));
                        }
                    }
                }
            }
        }

        public static T GetObject<T>(Page p)
        {
            return (T)GetObject(typeof(T), p);
        }

        public static object GetObject(Type t, Page p)
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(t);
            object obj = oi.NewObject();
            EnumControls(p, oi, delegate(MemberHandler h, Control c)
            {
                string v = GetValue(c);
                if (h.FieldType.IsEnum)
                {
                    int n = (int)Enum.Parse(h.FieldType, v);
                    h.SetValue(obj, n);
                }
                else
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        if (h.AllowNull)
                        {
                            h.SetValue(obj, null);
                        }
                        else
                        {
                            if (h.FieldType == typeof(string))
                            {
                                h.SetValue(obj, "");
                            }
                        }
                    }
                    else
                    {
                        h.SetValue(obj, Convert.ChangeType(v, h.FieldType));
                    }
                }
            });
            return obj;
        }

        private static string GetValue(Control c)
        {
            if (c is TextBox)
            {
                return ((TextBox)c).Text;
            }
            if (c is CheckBox)
            {
                return ((CheckBox)c).Checked.ToString();
            }
            if (c is DropDownList)
            {
                return ((DropDownList)c).SelectedValue;
            }
            throw new NotSupportedException();
        }

        public static void SetObject(object obj, Page p)
        {
            Type t = obj.GetType();
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(t);
            EnumControls(p, oi, delegate(MemberHandler h, Control c)
            {
                object v = h.GetValue(obj);
                SetValue(c, v);
            });
        }

        private static void SetValue(Control c, object v)
        {
            if (c is TextBox)
            {
                ((TextBox)c).Text = (v ?? "").ToString();
            }
            else if (c is CheckBox)
            {
                ((CheckBox)c).Checked = (bool)v;
            }
            else if (c is DropDownList)
            {
                Type t = v.GetType();
                ((DropDownList)c).SelectedValue = v.ToString();
            }
            else throw new NotSupportedException();
        }
    }
}
