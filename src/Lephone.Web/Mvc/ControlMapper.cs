using System;
using Lephone.Data.Common;
using Lephone.Core.Text;

namespace Lephone.Web.Mvc
{
    public class ControlMapper
    {
        private readonly string id;
        private readonly string name;
        private readonly HtmlBuilder b;

        public static HtmlBuilder Map(MemberHandler m, string id, string name, object value)
        {
            var c = new ControlMapper(m, id, name, value);
            return c.b;
        }

        public ControlMapper(MemberHandler m, string id, string name, object value)
        {
            this.id = id;
            this.name = name;

            b = HtmlBuilder.New;

            if(m.IsLazyLoad)
            {
                ProcessField(m.FieldType.GetGenericArguments()[0], m, value);
            }
            else
            {
                ProcessField(m.FieldType, m, value);
            }
        }

        private void ProcessField(Type FieldType, MemberHandler m, object value)
        {
            if (FieldType.IsEnum)
            {
                ProcessEnum(m, value);
            }
            else if (FieldType == typeof(bool))
            {
                ProcessBoolean(value);
            }
            else if (FieldType == typeof(string))
            {
                ProcessString(m, value);
            }
            else if (FieldType == typeof(DateTime))
            {
                ProcessDateTime(m, value);
            }
            else if (FieldType == typeof(Date))
            {
                ProcessDateTime(m, value);
            }
            else if (FieldType == typeof(Time))
            {
                ProcessDateTime(m, value);
            }
            else if (FieldType.IsValueType)
            {
                ProcessValueType(value);
            }
            else
            {
                throw new WebException(string.Format("Out of scope of member {0}", m.MemberInfo.Name));
            }
        }

        public override string ToString()
        {
            return b.ToString();
        }

        private void ProcessEnum(MemberHandler m, object value)
        {
            string v = (value == null) ? "" : value.ToString();
            if(value != null && value.GetType() == typeof(string))
            {
                v = "";
            }
            b.tag("select").id(id).name(name);
            foreach (string s in Enum.GetNames(m.FieldType))
            {
                object e = Enum.Parse(m.FieldType, s);
                b.tag("option").attr("value", s);
                if (s == v)
                {
                    b.attr("selected", "true");
                }
                b.text(StringHelper.EnumToString(e)).end.over();
            }
            b.end.over();
            if (value != null && value.GetType() == typeof(string))
            {
                b.include("<script type=\"text/javascript\">document.getElementById(\"")
                    .include(id)
                    .include("\").value = '")
                    .include(value.ToString())
                    .include("';</script>");
            }
        }

        private void ProcessBoolean(object value)
        {
            b.input.id(id).name(name).type("checkbox");
            if (value != null)
            {
                if(value.GetType() == typeof(string))
                {
                    var s = (string)value;
                    s = s.Substring(0, s.Length - 2) + "? \"checked\" : \"\" " + s.Substring(s.Length - 2);
                    b.attr(s, null);
                }
                else
                {
                    if ((bool)value)
                    {
                        b.attr("checked", null);
                    }
                }
            }
            b.end.over();
        }

        private void ProcessString(MemberHandler m, object value)
        {
            if (m.MaxLength < 256 && m.MaxLength > 0)
            {
                b.input.id(id).name(name).type("text");
                int size = m.MaxLength > 100 ? 100 : m.MaxLength;
                b.attr("maxlength", m.MaxLength).attr("size", size);
                if (value != null)
                {
                    b.value(value);
                }
                b.end.over();
            }
            else
            {
                string s = (value == null) ? "" : value.ToString();
                b.tag("textarea").id(id).name(name).attr("cols", 50).attr("rows", 5).include(s).end.over();
            }
        }

        private void ProcessDateTime(MemberHandler m, object value)
        {
            b.input.id(id).name(name).type("text").attr("cols", 19).attr("onclick", "PickDate(this)");
            if (m.IsAutoSavedValue)
            {
                b.attr("disabled", "true");
            }
            if (value != null)
            {
                b.value(value);
            }
            b.end.over();
        }

        private void ProcessValueType(object value)
        {
            b.input.id(id).name(name).attr("maxlength", 20).attr("cols", 20);
            if (value != null)
            {
                b.value(value);
            }
            b.end.over();
        }
    }
}


