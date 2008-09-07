using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lephone.Util;
using Lephone.Data;
using Lephone.Data.Common;
using System.Collections.Generic;
using Lephone.Util.Text;

namespace Lephone.Web.Common
{
    public static class PageHelper
    {
        public static bool ValidateSave(Page p, object obj, NoticeLabel msg, string NoticeText)
        {
            var vh = new ValidateHandler();
            return ValidateSave(p, vh, obj, msg, NoticeText, "ErrInput");
        }

        public static bool ValidateSave(Page p, ValidateHandler vh, object obj, NoticeLabel msg, string NoticeText, string CssErrInput)
        {
            return ValidateSave(p, vh, obj, msg, NoticeText, CssErrInput, () => DbEntry.Save(obj));
        }

        public static bool ValidateSave(Page p, ValidateHandler vh, object obj, NoticeLabel msg, string NoticeText,
            string CssErrInput, CallbackVoidHandler callback)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(obj.GetType());
            EnumControls(p, oi, delegate(MemberHandler mh, WebControl c)
            {
                c.CssClass = "";
            });
            vh.ValidateObject(obj);
            if (vh.IsValid)
            {
                callback();
                if (msg != null)
                {
                    msg.AddNotice(NoticeText);
                }
            }
            else
            {
                foreach (string str in vh.ErrorMessages.Keys)
                {
                    if (msg != null)
                    {
                        msg.AddWarning(vh.ErrorMessages[str]);
                    }
                    WebControl c = GetWebControl(p, oi, str);
                    if (c != null)
                    {
                        c.CssClass = CssErrInput;
                    }
                }
            }
            return vh.IsValid;
        }

        private static WebControl GetWebControl(Page p, ObjectInfo oi, string Name)
        {
            string cid = string.Format("{0}_{1}", oi.BaseType.Name, Name);
            var c = ClassHelper.GetValue(p, cid) as WebControl;
            return c;
        }

        private static void EnumControls(Page p, ObjectInfo oi, CallbackObjectHandler2<MemberHandler, WebControl> callback)
        {
            foreach (MemberHandler h in oi.SimpleFields)
            {
                if (!h.IsKey)
                {
                    string cid = string.Format("{0}_{1}", oi.BaseType.Name, h.MemberInfo.Name);
                    var c = ClassHelper.GetValue(p, cid) as WebControl;
                    if (c != null)
                    {
                        callback(h, c);
                    }
                    else
                    {
                        if (!h.IsAutoSavedValue && !h.AllowNull)
                        {
                            throw new DataException(string.Format("Control {0} not find!", cid));
                        }
                    }
                }
            }
        }

        public static T GetObject<T>(Page p, string ParseErrorText)
        {
            return (T)GetObject(typeof(T), p, ParseErrorText);
        }

        public static object GetObject(Type t, Page p, string ParseErrorText)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            object obj = oi.NewObject();
            EnumControls(p, oi, delegate(MemberHandler h, WebControl c)
            {
                string v = GetValue(c);
                if (h.FieldType.IsEnum)
                {
                    var n = (int)Enum.Parse(h.FieldType, v);
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
                            else if(!h.IsCreatedOn)
                            {
                                throw new WebControlException(c, string.Format(ParseErrorText, h.Name, ""));
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            object iv = ClassHelper.ChangeType(v, h.FieldType);
                            h.SetValue(obj, iv);
                        }
                        catch (Exception ex)
                        {
                            throw new WebControlException(c, string.Format(ParseErrorText, h.Name, ex.Message));
                        }
                    }
                }
            });
            return obj;
        }

        private static string GetValue(WebControl c)
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
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            EnumControls(p, oi, delegate(MemberHandler h, WebControl c)
            {
                object v = h.GetValue(obj);
                SetValue(c, v);
            });
        }

        private static void SetValue(WebControl c, object v)
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
                // Type t = v.GetType();
                ((DropDownList)c).SelectedValue = v.ToString();
            }
            else if (c is Label)
            {
                ((Label)c).Text = (v ?? "").ToString();
            }
            else throw new NotSupportedException();
        }

        public static ListItem[] GetItems(Type EnumType)
        {
            if (!EnumType.IsEnum) throw new ArgumentOutOfRangeException();

            var ret = new List<ListItem>();
            foreach (string v in Enum.GetNames(EnumType))
            {
                string n = StringHelper.EnumToString(EnumType, v);
                var li = new ListItem(n, v);
                ret.Add(li);
            }
            return ret.ToArray();
        }
    }
}
