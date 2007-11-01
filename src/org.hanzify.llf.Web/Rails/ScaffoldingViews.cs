
using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Reflection;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    internal class ScaffoldingViews : PageBase
    {
        private ObjectInfo oi;
        private HttpContext ctx;

        private static readonly string tHeader = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
<title>Edit</title>
<link href=""{0}/styles/scaffolding.css"" type=""text/css"" rel=""Stylesheet"" />
</head>
<body>

";

        private static readonly string tFooter = @"
</body>
</html>";

        public ScaffoldingViews(Type t, HttpContext context)
        {
            oi = ObjectInfo.GetInstance(t);
            ctx = context;
        }

        public override void ProcessRequest(HttpContext context)
        {
            MethodInfo mi = typeof(ScaffoldingViews).GetMethod(ActionName, ClassHelper.InstancePublic | BindingFlags.IgnoreCase);
            if (mi != null)
            {
                mi.Invoke(this, new object[] { });
            }
            else
            {
                throw new WebException(string.Format("View {0} not found in {1}!", ActionName, ControllerName));
            }
        }

        private void Process(CallbackObjectHandler<HtmlBuilder> callback)
        {
            ctx.Response.Write(string.Format(tHeader, ctx.Request.ApplicationPath));

            HtmlBuilder b = HtmlBuilder.New;

            callback(b);

            ctx.Response.Write(b);

            ctx.Response.Write(tFooter);
        }

        public void Edit()
        {
            Process(delegate(HtmlBuilder b)
            {
                string cn = oi.BaseType.Name;
                object o = bag["item"];
                object id = oi.Handler.GetKeyValue(o);
                b.h1.text(cn + " Edit").end.enter();
                b.form("post", UrlTo(ctx.Request.ApplicationPath, ControllerName, "update", id.ToString())).enter();

                foreach (MemberHandler m in oi.SimpleFields)
                {
                    if (!m.IsDbGenerate && !m.IsCreatedOn && !m.IsUpdatedOn)
                    {
                        string n = cn + "_" + m.Name;
                        string n1 = cn + "[" + m.Name + "]";
                        b.p.label.attr("for", n).text(m.Name).end.br.include(ControlMapper.Map(m, n, n1)).end.enter();
                    }
                }
                b.input.name("commit").type("submit").value("Update").end.enter().end.enter().enter();

                b.include(LinkTo(ctx.Request.ApplicationPath, "Show", null, "show", id, null)).enter();
                b.include(LinkTo(ctx.Request.ApplicationPath, "Back", null, "list", null, null)).enter();
            });
        }

        public void List()
        {
            Process(delegate(HtmlBuilder b)
            {
                string cn = oi.BaseType.Name;
                b.p.style("color: Green").text(flash["notice"]).end.enter().enter();
                b.h1.text("Listing " + cn).end.enter().enter();

                b.table.tr.enter();

                foreach (MemberHandler m in oi.SimpleFields)
                {
                    b.th.text(m.Name).end.enter();
                }

                b.end.enter().enter();

                foreach (object o in bag["list"] as IEnumerable)
                {
                    b.tr.over();
                    object id = oi.Handler.GetKeyValue(o);
                    foreach (MemberHandler m in oi.SimpleFields)
                    {
                        b.td.text(m.GetValue(o) ?? "<NULL>").end.enter();
                    }
                    b.td.include(LinkTo(ctx.Request.ApplicationPath, "Show", null, "show", id, null)).end.enter();
                    b.td.include(LinkTo(ctx.Request.ApplicationPath, "Edit", null, "edit", id, null)).end.enter();
                    b.td.include(LinkTo(ctx.Request.ApplicationPath, "Destroy", null, "destroy", id, "onclick=\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\"")).end.enter();
                    b.end.enter().enter();
                }

                b.end.enter().enter();

                int count = (int)(long)bag["list_count"];
                int pagesize = (int)bag["list_pagesize"];
                for (int i = 0, n = 1; i < count; n++, i += pagesize)
                {
                    b.include(LinkTo(ctx.Request.ApplicationPath, n.ToString(), null, "list", n.ToString(), null)).enter();
                }

                b.enter().br.br.include(LinkTo(ctx.Request.ApplicationPath, "New " + cn, null, "new", null, null)).br.enter();
            });
        }

        public void New()
        {
            Process(delegate(HtmlBuilder b)
            {
                string cn = oi.BaseType.Name;
                b.h1.text("New " + cn).end.enter();
                b.form("post", UrlTo(ctx.Request.ApplicationPath, ControllerName, "create", null)).enter();

                foreach (MemberHandler m in oi.SimpleFields)
                {
                    if (!m.IsDbGenerate && !m.IsCreatedOn && !m.IsUpdatedOn)
                    {
                        string n = cn + "_" + m.Name;
                        string n1 = cn + "[" + m.Name + "]";
                        b.p.label.attr("for", n).text(m.Name).end.br.include(ControlMapper.Map(m, n, n1)).end.enter();
                    }
                }

                b.input.name("commit").type("submit").value("Create").end.enter().end.enter().enter();

                b.include(LinkTo(ctx.Request.ApplicationPath, "Back", null, "list", null, null)).enter();
            });
        }

        public void Show()
        {
            Process(delegate(HtmlBuilder b)
            {
                object o = bag["item"];
                object id = oi.Handler.GetKeyValue(o);

                b.p.style("color: Green").text(flash["notice"]).end.enter().enter();

                foreach (MemberHandler m in oi.SimpleFields)
                {
                    b.p.tag("b").text(m.Name + ":").end.include(" ").text(m.GetValue(o) ?? "<NULL>").end.enter();
                }

                b.enter();

                b.include(LinkTo(ctx.Request.ApplicationPath, "Edit", null, "edit", id, null)).enter();
                b.include(LinkTo(ctx.Request.ApplicationPath, "Back", null, "list", null, null)).enter();
            });
        }

        public void Create() { }

        public void Update() { }

        public void Destroy() { }
    }
}
