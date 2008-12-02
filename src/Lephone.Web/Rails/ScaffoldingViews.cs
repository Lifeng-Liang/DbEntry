using System;
using System.Collections;
using System.Web;
using System.Reflection;
using Lephone.Data.Common;
using Lephone.Util;
using Lephone.Util.Text;

namespace Lephone.Web.Rails
{
    internal class ScaffoldingViews : PageBase
    {
        private readonly ObjectInfo oi;
        private readonly HttpContext ctx;

        private const string tHeader = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
<title>Edit</title>
<script type=""text/javascript"" language=""javascript"" src=""{0}/scripts/scaffolding.js""></script>
<link href=""{0}/styles/scaffolding.css"" type=""text/css"" rel=""Stylesheet"" />
</head>
<body>

";

        private const string tFooter = @"
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
                b.form("post",
                       UrlTo(ctx.Request.ApplicationPath, new UTArgs {Controller = ControllerName, Action = "update"},
                             id.ToString())).enter();

                foreach (MemberHandler m in oi.Fields)
                {
                    if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                    {
                        string n = cn + "_" + m.Name;
                        string n1 = cn + "[" + m.Name + "]";
                        object v = m.GetValue(o);
                        if(m.IsLazyLoad)
                        {
                            v = m.FieldType.GetProperty("Value").GetValue(v, null);
                        }
                        b.p.label.attr("for", n).text(m.Name).end.br.include(ControlMapper.Map(m, n, n1, v)).end.enter();
                    }
                }
                b.input.name("commit").type("submit").value("Update").end.enter().end.enter().enter();

                b.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs{Title = "Show", Action = "show"}, id)).enter();
                b.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs{Title = "Back", Action = "list"})).enter();
            });
        }

        public void List()
        {
            Process(delegate(HtmlBuilder b)
            {
                string cn = oi.BaseType.Name;
                b.p.style("color: Green").text(flash["notice"]).end.enter().enter();
                b.h1.text("Listing " + Inflector.Pluralize(cn)).end.enter().enter();

                b.table.tr.enter();

                foreach (MemberHandler m in oi.SimpleFields)
                {
                    b.th.text(m.Name).end.enter();
                }

                b.end.enter().enter();

                var objlist = bag["list"] as IEnumerable;
                if(objlist != null)
                {
                    foreach (object o in objlist)
                    {
                        b.tr.over();
                        object id = oi.Handler.GetKeyValue(o);
                        foreach (MemberHandler m in oi.SimpleFields)
                        {
                            b.td.text(m.GetValue(o) ?? "<NULL>").end.enter();
                        }
                        b.td.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs {Title = "Show", Action = "show"}, id)).end.enter();
                        b.td.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs {Title = "Edit", Action = "edit"}, id)).end.enter();
                        b.td.include(
                            LinkTo(ctx.Request.ApplicationPath, new LTArgs { Title = "Destroy", Action = "destroy", 
                                    Addon = "onclick=\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\"" },
                                id)).end.enter();
                        b.end.enter().enter();
                    }

                    b.end.enter().enter();

                    var count = (int)(long)bag["list_count"];
                    var pagesize = (int)bag["list_pagesize"];
                    for (int i = 0, n = 1; i < count; n++, i += pagesize)
                    {
                        b.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs{Title = n.ToString(), Action = "list"}, n)).enter();
                    }

                    b.enter().br.br.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs {Title = "New " + cn, Action = "new"})).br.enter();
                }
            });
        }

        public void New()
        {
            Process(delegate(HtmlBuilder b)
            {
                string cn = oi.BaseType.Name;
                b.h1.text("New " + cn).end.enter();
                b.form("post", UrlTo(ctx.Request.ApplicationPath, new UTArgs {Controller = ControllerName, Action = "create"})).enter();

                foreach (MemberHandler m in oi.Fields)
                {
                    if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                    {
                        string n = cn + "_" + m.Name;
                        string n1 = cn + "[" + m.Name + "]";
                        b.p.label.attr("for", n).text(m.Name).end.br.include(ControlMapper.Map(m, n, n1, null)).end.enter();
                    }
                }

                b.input.name("commit").type("submit").value("Create").end.enter().end.enter().enter();

                b.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs{Title = "Back", Action = "list"})).enter();
            });
        }

        public void Show()
        {
            Process(delegate(HtmlBuilder b)
            {
                object o = bag["item"];
                object id = oi.Handler.GetKeyValue(o);

                b.p.style("color: Green").text(flash["notice"]).end.enter().enter();

                foreach (MemberHandler m in oi.Fields)
                {
                    if(!m.IsRelationField)
                    {
                        object v = m.GetValue(o);
                        if(m.IsLazyLoad)
                        {
                            v = m.FieldType.GetProperty("Value").GetValue(v, null);
                        }
                        b.p.tag("b").text(m.Name + ":").end.include(" ").text(v ?? "<NULL>").end.enter();
                    }
                }

                b.enter();

                b.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs{Title = "Edit", Action = "edit"}, id)).enter();
                b.include(LinkTo(ctx.Request.ApplicationPath, new LTArgs{Title = "Back", Action = "list"})).enter();
            });
        }

        public void Create() { }

        public void Update() { }

        public void Destroy() { }
    }
}
