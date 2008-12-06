using System;
using Lephone.Data.Common;
using Lephone.Web;
using Lephone.Util.Text;

namespace Lephone.CodeGen
{
    public class AspNetGenerator
    {
        private readonly Type type;
        private readonly ObjectInfo oi;
        private readonly HtmlBuilder b;
        string title, id;

        public AspNetGenerator(Type type)
        {
            this.type = type;
            oi = ObjectInfo.GetInstance(type);
            b = HtmlBuilder.New.table.attr("border", 0).enter();
        }

        public override string ToString()
        {
            string oibtName = oi.BaseType.Name;
            foreach (var m in oi.SimpleFields)
            {
                if (m.IsKey)
                {
                    continue;
                }

                string memberName = m.MemberInfo.Name;

                title = oibtName + " " + memberName + ":";
                id = oibtName + "_" + memberName;

                b.tr.td.Class("FieldTitle").text(title).end.td.Class("FieldControl");

                ProcessMember(m);

                b.end.end.enter();
            }
            b.end.enter();
            return b.ToString();
        }

        private void ProcessMember(MemberHandler m)
        {
            Type mft = m.FieldType;
            if (mft.IsEnum)
            {
                ProcessEnum(m);
            }
            else if (mft == typeof(bool))
            {
                ProcessBoolean(m);
            }
            else if (mft == typeof(string))
            {
                ProcessString(m);
            }
            else if (mft == typeof(DateTime))
            {
                ProcessDateTime(m);
            }
            else if (mft.IsValueType)
            {
                ProcessValueType(m);
            }
            else
            {
                throw new ApplicationException(string.Format("Out of scope of member {0} of class: {1}", m.MemberInfo.Name, type.FullName));
            }
        }

        protected void ProcessEnum(MemberHandler m)
        {
            Type t = m.FieldType;
            b.asp("DropDownList", id);

            foreach (string s in Enum.GetNames(t))
            {
                object e = Enum.Parse(t, s);
                string text = StringHelper.EnumToString(e);
                b.tag("asp:ListItem").attr("Text", text).attr("Value", s).end.over();
            }
            b.end.over();
        }

        protected void ProcessBoolean(MemberHandler m)
        {
            b.asp("CheckBox", id).end.over();
        }

        protected void ProcessString(MemberHandler m)
        {
            int Iml = m.MaxLength;
            b.asp("TextBox", id);
            if (Iml > 0)
            {
                b.attr("MaxLength", Iml).attr("Columns", Iml > 60 ? 60 : Iml);
            }
            else
            {
                b.attr("TextMode", "MultiLine").attr("Columns", 60).attr("Rows", 10);
            }
            b.end.over();
        }

        protected void ProcessDateTime(MemberHandler m)
        {
            b.asp("TextBox", id).attr("Columns", 23);

            if (m.IsCreatedOn || m.IsUpdatedOn)
            {
                b.attr("Enabled", "False");
            }
            else
            {
                b.attr("onclick", "getDateString(this, oCalendarEn)");
            }
            b.end.over();
        }

        protected void ProcessValueType(MemberHandler m)
        {
            b.asp("TextBox", id).attr("MaxLength", 20).attr("Columns", 20).end.over();
        }
    }
}