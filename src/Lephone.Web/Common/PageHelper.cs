using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lephone.Core;
using Lephone.Data;
using System.Collections.Generic;
using Lephone.Core.Text;
using Lephone.Data.Definition;
using Lephone.Data.Model;
using Lephone.Data.Model.Member;

namespace Lephone.Web.Common
{
    public static class PageHelper
    {
        public static bool ValidateSave(Page p, IDbObject obj, NoticeLabel msg, string noticeText)
        {
            var vh = new ValidateHandler();
            return ValidateSave(p, vh, obj, msg, noticeText, "ErrInput");
        }

        public static bool ValidateSave(Page p, ValidateHandler vh, IDbObject obj, NoticeLabel msg, string noticeText, string cssErrInput)
        {
            return ValidateSave(p, vh, obj, msg, noticeText, cssErrInput, () => DbEntry.Save(obj));
        }

        public static bool ValidateSave(Page p, ValidateHandler vh, object obj, NoticeLabel msg, string noticeText,
            string cssErrInput, CallbackVoidHandler callback)
        {
            var ctx = ModelContext.GetInstance(obj.GetType());
            EnumControls(p, ctx.Info, delegate(MemberHandler mh, WebControl c)
            {
                c.CssClass = "";
            });
            vh.ValidateObject(obj);
            if (vh.IsValid)
            {
                callback();
                if (msg != null)
                {
                    msg.AddNotice(noticeText);
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
                    WebControl c = GetWebControl(p, ctx.Info, str);
                    if (c != null)
                    {
                        c.CssClass = cssErrInput;
                    }
                }
            }
            return vh.IsValid;
        }

        private static WebControl GetWebControl(Page p, ObjectInfo oi, string name)
        {
            string cid = string.Format("{0}_{1}", oi.HandleType.Name, name);
            var c = ClassHelper.GetValue(p, cid) as WebControl;
            return c;
        }

        private static void EnumControls(Page p, ObjectInfo oi, CallbackObjectHandler2<MemberHandler, WebControl> callback)
        {
            foreach (MemberHandler h in oi.SimpleMembers)
            {
                if (!h.Is.Key)
                {
                    string cid = string.Format("{0}_{1}", oi.HandleType.Name, h.MemberInfo.Name);
                    var c = ClassHelper.GetValue(p, cid) as WebControl;
                    if (c != null)
                    {
                        callback(h, c);
                    }
                    else
                    {
                        if (!h.Is.AutoSavedValue && !h.Is.AllowNull)
                        {
                            throw new DataException(string.Format("Control {0} not find!", cid));
                        }
                    }
                }
            }
        }

        public static T GetObject<T>(Page p, string parseErrorText)
        {
            return (T)GetObject(typeof(T), p, parseErrorText);
        }

        public static object GetObject(Type t, Page p, string parseErrorText)
        {
            var ctx = ModelContext.GetInstance(t);
            object obj = ctx.NewObject();
            EnumControls(p, ctx.Info, delegate(MemberHandler h, WebControl c)
            {
                string v = GetValue(c);
                if (h.MemberType.IsEnum)
                {
                    var n = (int)Enum.Parse(h.MemberType, v);
                    h.SetValue(obj, n);
                }
                else
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        if (h.Is.AllowNull)
                        {
                            h.SetValue(obj, null);
                        }
                        else
                        {
                            if (h.MemberType == typeof(string))
                            {
                                h.SetValue(obj, "");
                            }
                            else if(!h.Is.CreatedOn && !h.Is.SavedOn)
                            {
                                throw new WebControlException(c, string.Format(parseErrorText, h.Name, ""));
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            object iv = ClassHelper.ChangeType(v, h.MemberType);
                            h.SetValue(obj, iv);
                        }
                        catch (Exception ex)
                        {
                            throw new WebControlException(c, string.Format(parseErrorText, h.Name, ex.Message));
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
            if(c is Label)
            {
                return ((Label) c).Text;
            }
            return GetPropertyInfo(c).GetValue(c, null).ToString();
        }

        private static PropertyInfo GetPropertyInfo(object c)
        {
            var attr = c.GetType().GetAttribute<DefaultPropertyAttribute>(false);
            if (attr != null)
            {
                var pi = c.GetType().GetProperty(attr.Name);
                if (pi != null)
                {
                    return pi;
                }
            }
            throw new NotSupportedException();
        }

        public static void SetObject(object obj, Page p)
        {
            Type t = obj.GetType();
            var ctx = ModelContext.GetInstance(t);
            EnumControls(p, ctx.Info, delegate(MemberHandler h, WebControl c)
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
            else GetPropertyInfo(c).SetValue(c, v, null);
        }

        public static ListItem[] GetItems(Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentOutOfRangeException();

            var ret = new List<ListItem>();
            foreach (string v in Enum.GetNames(enumType))
            {
                string n = StringHelper.EnumToString(enumType, v);
                var li = new ListItem(n, v);
                ret.Add(li);
            }
            return ret.ToArray();
        }
    }
}
