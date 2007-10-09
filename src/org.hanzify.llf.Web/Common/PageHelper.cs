
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
            ValidateSave(vh, obj, msg, NoticeText);
        }
        
        public static void ValidateSave(ValidateHandler vh, object obj, Label msg, string NoticeText)
        {
            vh.ValidateObject(obj);
            if (vh.IsValid)
            {
                DbEntry.Save(obj);
                msg.CssClass = "Notice";
                msg.Text = HttpUtility.HtmlEncode(NoticeText);
                msg.Visible = true;
            }
            else
            {
                msg.CssClass = "Warning";
                StringBuilder text = new StringBuilder("<ul>");
                foreach (string str in vh.ErrorMessages.Keys)
                {
                    text.Append("<li>").Append(HttpUtility.HtmlEncode(vh.ErrorMessages[str])).Append("</li>");
                }
                text.Append("</ul>");
                msg.Text = text.ToString();
                msg.Visible = true;
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
                h.SetValue(obj, Convert.ChangeType(GetValue(c), h.FieldType.IsEnum ? typeof(Int32) : h.FieldType));
            });
            return obj;
        }

        private static object GetValue(Control c)
        {
            if (c is TextBox)
            {
                return ((TextBox)c).Text;
            }
            if (c is CheckBox)
            {
                return ((CheckBox)c).Checked;
            }
            if (c is DropDownList)
            {
                return ((DropDownList)c).SelectedIndex;
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
                ((TextBox)c).Text = v.ToString();
            }
            else if (c is CheckBox)
            {
                ((CheckBox)c).Checked = (bool)v;
            }
            else if (c is DropDownList)
            {
                ((DropDownList)c).SelectedIndex = (int)v;
            }
            else throw new NotSupportedException();
        }
    }
}
