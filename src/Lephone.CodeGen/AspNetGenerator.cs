using System;
using Lephone.Data.Common;
using Lephone.Web;
using Lephone.Util.Text;

namespace Lephone.CodeGen
{
    public class AspNetGenerator
    {
        private readonly Type _type;
        private readonly ObjectInfo _oi;
        private readonly HtmlBuilder _b;
        string _title, _id;

        public AspNetGenerator(Type type)
        {
            this._type = type;
            _oi = ObjectInfo.GetInstance(type);
            _b = HtmlBuilder.New.table.attr("border", 0).enter();
        }

        public override string ToString()
        {
            string oibtName = _oi.BaseType.Name;
            foreach (var m in _oi.SimpleFields)
            {
                if (m.IsKey)
                {
                    continue;
                }

                string memberName = m.MemberInfo.Name;

                _title = oibtName + " " + memberName + ":";
                _id = oibtName + "_" + memberName;

                _b.tr.td.Class("FieldTitle").text(_title).end.td.Class("FieldControl");

                ProcessMember(m);

                _b.end.end.enter();
            }
            _b.end.enter();
            return _b.ToString();
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
                throw new ApplicationException(string.Format("Out of scope of member {0} of class: {1}", m.MemberInfo.Name, _type.FullName));
            }
        }

        protected void ProcessEnum(MemberHandler m)
        {
            Type t = m.FieldType;
            _b.asp("DropDownList", _id);

            foreach (string s in Enum.GetNames(t))
            {
                object e = Enum.Parse(t, s);
                string text = StringHelper.EnumToString(e);
                _b.tag("asp:ListItem").attr("Text", text).attr("Value", s).end.over();
            }
            _b.end.over();
        }

        protected void ProcessBoolean(MemberHandler m)
        {
            _b.asp("CheckBox", _id).end.over();
        }

        protected void ProcessString(MemberHandler m)
        {
            int iml = m.MaxLength;
            _b.asp("TextBox", _id);
            if (iml > 0)
            {
                _b.attr("MaxLength", iml).attr("Columns", iml > 60 ? 60 : iml);
            }
            else
            {
                _b.attr("TextMode", "MultiLine").attr("Columns", 60).attr("Rows", 10);
            }
            _b.end.over();
        }

        protected void ProcessDateTime(MemberHandler m)
        {
            _b.asp("TextBox", _id).attr("Columns", 23);

            if (m.IsCreatedOn || m.IsUpdatedOn)
            {
                _b.attr("Enabled", "False");
            }
            else
            {
                _b.attr("onclick", "getDateString(this, oCalendarEn)");
            }
            _b.end.over();
        }

        protected void ProcessValueType(MemberHandler m)
        {
            _b.asp("TextBox", _id).attr("MaxLength", 20).attr("Columns", 20).end.over();
        }
    }
}