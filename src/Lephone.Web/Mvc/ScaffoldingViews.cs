using System;
using System.Collections;
using System.Web;
using System.Reflection;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Core;
using Lephone.Core.Text;

namespace Lephone.Web.Mvc
{
    internal class ScaffoldingViews : PageBase
    {
        private readonly ModelContext _dctx;
        private readonly HttpContext _ctx;

        private const string HeaderTemplate = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
<title>Edit</title>
<script type=""text/javascript"" language=""javascript"" src=""{0}/scripts/scaffolding.js""></script>
<link href=""{0}/styles/scaffolding.css"" type=""text/css"" rel=""Stylesheet"" />
</head>
<body>

";

        private const string FooterTemplate = @"
</body>
</html>";

        private readonly ListStyle _style;

        public ScaffoldingViews(ControllerInfo ci, Type t, HttpContext context)
        {
            _style = ci.ListStyle;
            _dctx = ModelContext.GetInstance(t);
            _ctx = context;
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

        protected void BaseProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);
        }

        protected virtual void Process(CallbackObjectHandler<HtmlBuilder> callback)
        {
            _ctx.Response.Write(string.Format(HeaderTemplate, _ctx.Request.ApplicationPath));

            HtmlBuilder b = HtmlBuilder.New;

            callback(b);

            _ctx.Response.Write(b);

            _ctx.Response.Write(FooterTemplate);
        }

        public void Edit()
        {
            Process(delegate(HtmlBuilder b)
                    {
                        string cn = _dctx.Info.HandleType.Name;
                        object o = this["Item"];
                        object id = _dctx.Handler.GetKeyValue(o);
                        b.h1.text(cn + " Edit").end.enter();
                        b.form("post", UrlTo.Action("update").Parameters(id)).enter();

                        foreach (MemberHandler m in _dctx.Info.Fields)
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

                        b.include(LinkTo.Title("Show").Action("show").Parameters(id)).enter();
                        b.include(LinkTo.Title("Back").Action("list")).enter();
                    });
        }

        public void List()
        {
            Process(delegate(HtmlBuilder b)
                    {
                        string cn = _dctx.Info.HandleType.Name;
                        b.p.style("color: Green").text(Flash.Notice).end.enter().enter();
                        b.h1.text("Listing " + Inflector.Pluralize(cn)).end.enter().enter();

                        b.table.tr.enter();

                        foreach (MemberHandler m in _dctx.Info.SimpleFields)
                        {
                            b.th.text(m.Name).end.enter();
                        }

                        b.end.enter().enter();

                        var itemList = this["ItemList"];
                        if (itemList != null)
                        {
                            var objlist = (IEnumerable)ClassHelper.GetValue(itemList, "List");
                            foreach (object o in objlist)
                            {
                                b.tr.over();
                                object id = _dctx.Handler.GetKeyValue(o);
                                foreach (MemberHandler m in _dctx.Info.SimpleFields)
                                {
                                    b.td.text(m.GetValue(o) ?? "<NULL>").end.enter();
                                }
                                b.td.include(LinkTo.Title("Show").Action("show").Parameters(id)).end.enter();
                                b.td.include(LinkTo.Title("Edit").Action("edit").Parameters(id)).end.enter();
                                b.td.include(
                                    LinkTo.Title("Destroy").Action("destroy")
                                        .Addon("onclick=\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\"")
                                        .Parameters(id)).end.enter();
                                b.end.enter().enter();
                            }

                            b.end.enter().enter();

                            var pageCount = (long) ClassHelper.GetValue(itemList, "PageCount");

                            if(_style == ListStyle.Default)
                            {
                                for (long i = 1; i <= pageCount; i++)
                                {
                                    b.include(LinkTo.Title(i.ToString()).Action("list").Parameters(i)).enter();
                                }
                            }
                            else
                            {
                                for (long i = pageCount; i > 0; i--)
                                {
                                    b.include(LinkTo.Title(i.ToString()).Action("list").Parameters(i)).enter();
                                }
                            }

                            b.enter().br.br.include(LinkTo.Title("New " + cn).Action("new")).br.enter();
                        }
                    });
        }

        public void New()
        {
            Process(delegate(HtmlBuilder b)
                    {
                        string cn = _dctx.Info.HandleType.Name;
                        b.h1.text("New " + cn).end.enter();
                        b.form("post", UrlTo.Controller(ControllerName).Action("create")).enter();

                        foreach (MemberHandler m in _dctx.Info.Fields)
                        {
                            if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                            {
                                string n = cn + "_" + m.Name;
                                string n1 = cn + "[" + m.Name + "]";
                                b.p.label.attr("for", n).text(m.Name).end.br.include(ControlMapper.Map(m, n, n1, null)).end.enter();
                            }
                        }

                        b.input.name("commit").type("submit").value("Create").end.enter().end.enter().enter();

                        b.include(LinkTo.Title("Back").Action("list")).enter();
                    });
        }

        public void Show()
        {
            Process(delegate(HtmlBuilder b)
                    {
                        object o = this["Item"];
                        object id = _dctx.Handler.GetKeyValue(o);

                        b.p.style("color: Green").text(Flash.Notice).end.enter().enter();

                        foreach (MemberHandler m in _dctx.Info.Fields)
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

                        b.include(LinkTo.Title("Edit").Action("edit").Parameters(id)).enter();
                        b.include(LinkTo.Title("Back").Action("list")).enter();
                    });
        }

        public void Create() { }

        public void Update() { }

        public void Destroy() { }
    }
}


