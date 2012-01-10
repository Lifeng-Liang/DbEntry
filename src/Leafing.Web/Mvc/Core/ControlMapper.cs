using System;
using Leafing.Core.Text;
using Leafing.Data.Model.Member;

namespace Leafing.Web.Mvc.Core
{
    public class ControlMapper
    {
        private readonly string _id;
        private readonly string _name;
        private readonly HtmlBuilder _html;

        public static HtmlBuilder Map(MemberHandler m, string id, string name, object value)
        {
            var c = new ControlMapper(m, id, name, value);
            return c._html;
        }

        public ControlMapper(MemberHandler m, string id, string name, object value)
        {
            this._id = id;
            this._name = name;

            _html = HtmlBuilder.New;

            ProcessField(m.Is.LazyLoad ? m.MemberType.GetGenericArguments()[0] : m.MemberType, m, value);
        }

        private void ProcessField(Type fieldType, MemberHandler m, object value)
        {
            if (fieldType.IsEnum)
            {
                ProcessEnum(m, value);
            }
            else if (fieldType == typeof(bool))
            {
                ProcessBoolean(value);
            }
            else if (fieldType == typeof(string))
            {
                ProcessString(m, value);
            }
            else if (fieldType == typeof(DateTime))
            {
                ProcessDateTime(m, value);
            }
            else if (fieldType == typeof(Date))
            {
                ProcessDateTime(m, value);
            }
            else if (fieldType == typeof(Time))
            {
                ProcessDateTime(m, value);
            }
            else if (fieldType.IsValueType)
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
            return _html.ToString();
        }

        private void ProcessEnum(MemberHandler m, object value)
        {
            string v = (value == null) ? "" : value.ToString();
            if(value != null && value is string)
            {
                v = "";
            }
            _html.tag("select").id(_id).name(_name);
            foreach (string s in Enum.GetNames(m.MemberType))
            {
                object e = Enum.Parse(m.MemberType, s);
                _html.tag("option").attr("value", s);
                if (s == v)
                {
                    _html.attr("selected", "true");
                }
                _html.text(StringHelper.EnumToString(e)).end.over();
            }
            _html.end.over();
            if (value != null && value is string)
            {
                _html.include("<script type=\"text/javascript\">document.getElementById(\"")
                    .include(_id)
                    .include("\").value = '")
                    .include(value.ToString())
                    .include("';</script>");
            }
        }

        private void ProcessBoolean(object value)
        {
            _html.input.id(_id).name(_name).type("checkbox");
            if (value != null)
            {
                if(value is string)
                {
                    var s = (string)value;
                    s = s.Substring(0, s.Length - 2) + "? \"checked\" : \"\" " + s.Substring(s.Length - 2);
                    _html.attr(s, null);
                }
                else
                {
                    if ((bool)value)
                    {
                        _html.attr("checked", null);
                    }
                }
            }
            _html.end.over();
        }

        private void ProcessString(MemberHandler m, object value)
        {
            if (m.MaxLength < 256 && m.MaxLength > 0)
            {
                _html.input.id(_id).name(_name).type("text");
                int size = m.MaxLength > 100 ? 100 : m.MaxLength;
                _html.attr("maxlength", m.MaxLength).attr("size", size);
                if (value != null)
                {
                    _html.value(value);
                }
                _html.end.over();
            }
            else
            {
                string s = (value == null) ? "" : value.ToString();
                _html.tag("textarea").id(_id).name(_name).attr("cols", 50).attr("rows", 5).include(s).end.over();
            }
        }

        private void ProcessDateTime(MemberHandler m, object value)
        {
            _html.input.id(_id).name(_name).type("text").attr("cols", 19).attr("onclick", "PickDate(this)");
            if (m.Is.AutoSavedValue)
            {
                _html.attr("disabled", "true");
            }
            if (value != null)
            {
                _html.value(value);
            }
            _html.end.over();
        }

        private void ProcessValueType(object value)
        {
            _html.input.id(_id).name(_name).attr("maxlength", 20).attr("cols", 20);
            if (value != null)
            {
                _html.value(value);
            }
            _html.end.over();
        }
    }
}


