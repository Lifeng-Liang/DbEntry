using System;
using System.Text;
using Lephone.Data.Common;
using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Web;
using Lephone.Web.Rails;

namespace Lephone.CodeGen
{
    public class RailsViewGenerator
    {
        private const string tHeader = @"<%@ Page Language=""C#"" %>

<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
{0}
<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head runat=""server"">
  <title></title>
  <script type=""text/javascript"" language=""javascript"" src=""~/scripts/scaffolding.js""></script>
  <link href=""~/styles/scaffolding.css"" type=""text/css"" rel=""Stylesheet"" />
</head>
<body>

";

        private const string mHeader = @"<%@ Page Title="""" Language=""C#"" MasterPageFile=""{1}"" %>
{0}
<asp:Content ID=""Content1"" ContentPlaceHolderID=""head"" Runat=""Server"">
</asp:Content>
<asp:Content ID=""Content2"" ContentPlaceHolderID=""ContentPlaceHolder1"" Runat=""Server"">

";

        private const string tScript = @"
<script runat=""server"">{0}</script>
";

        private const string tFooter = @"
</body>
</html>";

        private const string mFooter = @"
</asp:Content>";

        private Type ClassType;
        private readonly string ViewName;
        private readonly string MasterPageName;

        public RailsViewGenerator(string fileName, string className, string viewName, string MasterPageName)
        {
            Helper.EnumTypes(fileName, t =>
            {
                if (t.FullName == className)
                {
                    ClassType = t;
                    return false;
                }
                return true;
            });
            ViewName = viewName;
            this.MasterPageName = MasterPageName;
        }

        public override string ToString()
        {
            switch(ViewName)
            {
                case "New":
                    return GetViewNew();
                case "List":
                    return GetViewList();
                case "Show":
                    return GetViewShow();
                case "Edit":
                    return GetViewEdit();
            }
            throw new ArgsErrorException(6, "View name invalid.");
        }

        private void Process(StringBuilder sb, string vars, CallbackObjectHandler<HtmlBuilder> callback)
        {
            string vs = string.IsNullOrEmpty(vars) ? "" : string.Format(tScript, vars);

            if(string.IsNullOrEmpty(MasterPageName))
            {
                sb.Append(string.Format(tHeader, vs));
            }
            else
            {
                sb.Append(string.Format(mHeader, vs, MasterPageName));
            }

            var b = HtmlBuilder.New;

            callback(b);

            sb.Append(b);

            if(string.IsNullOrEmpty(MasterPageName))
            {
                sb.Append(tFooter);
            }
            else
            {
                sb.Append(mFooter);
            }
        }


        private string GetViewNew()
        {
            var sb = new StringBuilder();
            Process(sb, null, delegate(HtmlBuilder b)
            {
                string cn = ClassType.Name;
                b.h1.text("New " + cn).end.enter().enter();
                b.form("post", "<%= UrlTo.Action(\"create\") %>").enter();

                var oi = ObjectInfo.GetInstance(ClassType);
                foreach (MemberHandler m in oi.Fields)
                {
                    if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                    {
                        string n = cn.ToLower() + "_" + m.Name.ToLower();
                        string n1 = cn.ToLower() + "[" + m.Name.ToLower() + "]";
                        b.include("  ").p.label.attr("for", n).text(m.Name).end.br.include(ControlMapper.Map(m, n, n1, null)).end.enter();
                    }
                }

                b.include("  ").input.name("commit").type("submit").value("Create").end.enter().end.enter().enter();

                b.include("<%= LinkTo.Title(\"Back\").Action(\"list\") %>").enter();
            });
            return sb.ToString();
        }

        private string GetViewList()
        {
            var sb = new StringBuilder();

            string vars = string.Format(@"
    public ItemList<{0}> ItemList;
", ClassType.Name);

            Process(sb, vars, delegate(HtmlBuilder b)
            {
                string cn = ClassType.Name;

                b.p.style("color: Green").include("<%= Flash.Notice %>").end.enter().enter();
                b.h1.text("Listing " + Inflector.Pluralize(cn)).end.enter().enter();

                b.table.enter().tr.enter();

                var oi = ObjectInfo.GetInstance(ClassType);
                foreach (MemberHandler m in oi.SimpleFields)
                {
                    b.include("  ").th.text(m.Name).end.enter();
                }

                b.end.enter();

                b.include("<% foreach (var o in ItemList.List) { %>");
                b.enter().tr.enter();
                foreach (MemberHandler m in oi.SimpleFields)
                {
                    b.include("  ").td.include("<%= o.").include(m.Name).include(" %>").end.enter();
                }
                b.include("  ").td.include("<%= LinkTo.Title(\"Show\").Action(\"show\").Parameters(o.Id) %>").end.enter();
                b.include("  ").td.include("<%= LinkTo.Title(\"Edit\").Action(\"edit\").Parameters(o.Id) %>").end.enter();
                b.include("  ").td.include("<%= LinkTo.Title(\"Destroy\").Action(\"destroy\").Addon(\"onclick=\\\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\\\"\").Parameters(o.Id) %>").end.enter();
                b.end.enter();
                b.include("<% } %>").enter().end.enter();

                b.include(@"
<% for (int i = 1; i <= ItemList.PageCount; i++) { %>
  &nbsp;<%= LinkTo.Title(i.ToString()).Action(""list"").Parameters(i) %>
<% } %>
");

                b.enter().br.br.enter().enter().include("<%= LinkTo.Title(\"New " + cn + "\").Action(\"new\") %>").br.enter();
            });
            return sb.ToString();
        }

        private string GetViewShow()
        {
            var sb = new StringBuilder();
            Process(sb, "\n    public " + ClassType.Name + " Item;\n", b =>
            {
                b.p.style("color: Green").include("<%= Flash.Notice %>").end.enter().enter();

                var oi = ObjectInfo.GetInstance(ClassType);
                foreach (MemberHandler m in oi.Fields)
                {
                    if (!m.IsRelationField)
                    {
                        b.p.tag("b").text(m.Name + ":").end.include("<%= Item.").include(m.Name).include(" %>").end.enter();
                    }
                }

                b.enter();

                b.include("<%= LinkTo.Title(\"Edit\").Action(\"edit\").Parameters(Item.Id) %>").enter();
                b.include("<%= LinkTo.Title(\"Back\").Action(\"list\") %>").enter();
            });
            return sb.ToString();
        }

        private string GetViewEdit()
        {
            var sb = new StringBuilder();
            Process(sb, "\n    public " + ClassType.Name + " Item;\n", b =>
            {
                b.h1.text(ClassType.Name + " Edit").end.enter().enter();
                b.form("post", "<%= UrlTo.Action(\"update\").Parameters(Item.Id) %>").enter();

                var oi = ObjectInfo.GetInstance(ClassType);
                foreach (MemberHandler m in oi.Fields)
                {
                    if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                    {
                        string n = ClassType.Name.ToLower() + "_" + m.Name.ToLower();
                        string n1 = ClassType.Name.ToLower() + "[" + m.Name.ToLower() + "]";
                        b.include("  ").p.label.attr("for", n).text(m.Name).end.br
                            .include(ControlMapper.Map(m, n, n1, "<%= Item." + m.Name + " %>")).end.enter();
                    }
                }
                b.input.name("commit").type("submit").value("Update").end.enter().end.enter().enter();

                b.include("<%= LinkTo.Title(\"Show\").Action(\"show\").Parameters(Item.Id) %>").enter();
                b.include("<%= LinkTo.Title(\"Back\").Action(\"list\") %>").enter();
            });
            return sb.ToString();
        }
    }
}
