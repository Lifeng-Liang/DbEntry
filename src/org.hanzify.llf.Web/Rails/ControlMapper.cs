
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Common;
using Lephone.Util.Text;

namespace Lephone.Web.Rails
{
    internal class ControlMapper
    {
        private string id, name;
        private HtmlBuilder b;

        public static HtmlBuilder Map(MemberHandler m, string id, string name)
        {
            ControlMapper c = new ControlMapper(m, id, name);
            return c.b;
        }

        public ControlMapper(MemberHandler m, string id, string name)
        {
            this.id = id;
            this.name = name;

            b = HtmlBuilder.New;

            if (m.FieldType.IsEnum)
            {
                ProcessEnum(m);
            }
            else if (m.FieldType == typeof(bool))
            {
                ProcessBoolean(m);
            }
            else if (m.FieldType == typeof(string))
            {
                ProcessString(m);
            }
            else if (m.FieldType == typeof(DateTime))
            {
                ProcessDateTime(m);
            }
            else if (m.FieldType.IsValueType)
            {
                ProcessValueType(m);
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

        private void ProcessEnum(MemberHandler m)
        {
            b.tag("select").id(id).name(name);
            foreach (string s in Enum.GetNames(m.FieldType))
            {
                object e = Enum.Parse(m.FieldType, s);
                b.tag("option").attr("value", s).text(StringHelper.EnumToString(e)).end.over();
            }
            b.end.over();
        }

        private void ProcessBoolean(MemberHandler m)
        {
            b.input.id(id).name(name).type("checkbox").end.over();
        }

        private void ProcessString(MemberHandler m)
        {
            if (m.MaxLength < 50 && m.MaxLength > 0)
            {
                b.input.id(id).name(name).type("text");
                b.attr("maxlength", m.MaxLength).attr("size", m.MaxLength);
                b.end.over();
            }
            else
            {
                b.tag("textarea").id(id).name(name).attr("cols", 50).attr("rows", 5).text("").end.over();
            }
        }

        private void ProcessDateTime(MemberHandler m)
        {
            b.input.id(id).name(name).type("text").attr("cols", 19).attr("onclick", "getDateString(this, oCalendar)");
            if (m.IsCreatedOn || m.IsUpdatedOn)
            {
                b.attr("disabled", "true");
            }
            b.end.over();
        }

        private void ProcessValueType(MemberHandler m)
        {
            b.input.id(id).name(name).attr("maxlength", 20).attr("cols", 20).end.over();
        }
    }
}
