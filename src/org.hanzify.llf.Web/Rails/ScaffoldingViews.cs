
using System;
using System.Collections;
using System.Text;
using System.Web;
using Lephone.Data.Common;

namespace Lephone.Web.Rails
{
    internal class ScaffoldingViews : PageBase
    {
        private ObjectInfo oi;

        private static readonly string tHeader = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
<title>Edit</title>
<link href=""../../styles/scaffolding.css"" type=""text/css"" rel=""Stylesheet"" />
</head>
<body>";

        private static readonly string tFooter = @"
</body>
</html>";

        public ScaffoldingViews(Type t)
        {
            oi = ObjectInfo.GetInstance(t);
        }

        public void Edit()
        {
            Response.Write(tHeader);

            string cn = oi.BaseType.Name;
            object o = bag["item"];
            object id = oi.Handler.GetKeyValue(o);
            HtmlBuilder b = HtmlBuilder.New.h1.text(cn + " Edit").end;
            b.form.attr("action", UrlTo("update", id.ToString())).attr("method", "post");

            foreach (MemberHandler m in oi.SimpleFields)
            {
                string n = cn + "_" + m.Name;
                string n1 = cn + "[" + m.Name + "]";
                b.p.label.attr("for", n).text(m.Name).end.br.input.id(n).attr("name", n1).attr("size", 30).attr("type", "text").attr("value", m.GetValue(o)).end.end.enter.over();
            }
            b.input.attr("name", "commit").attr("type", "submit").attr("value", "Update").end.enter.end.enter.enter.over();

            b.a(LinkTo("Show", null, "show", id)).end.enter.over();
            b.a(LinkTo("Back", null, "list", null)).end.enter.enter.over();

            Response.Write(b);

            Response.Write(tFooter);
        }

        public void List()
        {
            Response.Write(tHeader);

            string cn = oi.BaseType.Name;
            HtmlBuilder b = HtmlBuilder.New.p.style("color: Green").text(flash["notice"]).end.enter.enter;
            b.h1.text("Listing " + cn).end.enter.enter.over();
            
            b.table.tr.over();

            foreach (MemberHandler m in oi.SimpleFields)
            {
                b.th.text(m.Name).end.over();
            }

            b.end.enter.over();

            foreach (object o in bag["list"] as IEnumerable)
            {
                b.tr.over();
                object id = oi.Handler.GetKeyValue(o);
                foreach (MemberHandler m in oi.SimpleFields)
                {
                    b.td.text(m.GetValue(o)).end.over();
                }
                b.td.text(LinkTo("Show", null, "show", id)).end.enter.over();
                b.td.text(LinkTo("Edit", null, "edit", id)).end.enter.over();
                b.td.text(LinkTo("Destroy", null, "destroy", id, "onclick=\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\"")).end.enter.over();
                b.end.over();
            }

            b.end.enter.over();

            int count = (int)(long)bag["list_count"];
            int pagesize = (int)bag["list_pagesize"];
            for (int i = 0, n = 1; i < count; n++, i += pagesize)
            {
                b.include(LinkTo(n.ToString(), null, "list", n.ToString()));
            }

            b.br.br.include(LinkTo("New User", null, "new", null)).br.enter.over();

            Response.Write(b);

            Response.Write(tFooter);
        }
    }
}
