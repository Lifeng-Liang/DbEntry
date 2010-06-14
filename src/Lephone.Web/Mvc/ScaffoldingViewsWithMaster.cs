using System;
using System.Web;
using System.Web.UI;
using Lephone.Core;

namespace Lephone.Web.Mvc
{
    internal class ScaffoldingViewsWithMaster : ScaffoldingViews
    {
        private HtmlBuilder _html;
        private static bool _initialized;
        private static object _fileDependencies;

        public ScaffoldingViewsWithMaster(ControllerInfo ci, Type t, HttpContext context)
            : base(ci, t, context)
        {
            base.AppRelativeVirtualPath = "~/ScaffoldingViews.aspx";
            if (!_initialized)
            {
                _fileDependencies = base.GetWrappedFileDependencies(new[] { WebSettings.ScaffoldingMasterPage });
                _initialized = true;
            }
        }

        public override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);
            // trick to call base.base.ProcessRequest, to avoid the ScaffoldingViewsBase class
            BaseProcessRequest(context);
        }

        protected override void Process(CallbackObjectHandler<HtmlBuilder> callback)
        {
            _html = HtmlBuilder.New;
            _html.div.Class("content").enter();
            callback(_html);
            _html.end.enter();
        }

        private void BuildControlContent1(Control ctrl)
        {
            IParserAccessor accessor = ctrl;
            accessor.AddParsedSubObject(new LiteralControl("\r\n"));
        }

        private void BuildControlContent2(Control ctrl)
        {
            IParserAccessor accessor = ctrl;
            accessor.AddParsedSubObject(new LiteralControl(_html.ToString()));
        }


        private void BuildControlTree()
        {
            Title = ActionName;
            MasterPageFile = WebSettings.ScaffoldingMasterPage;
            this.InitializeCulture();
            base.AddContentTemplate("head", new CompiledTemplateBuilder(BuildControlContent1));
            base.AddContentTemplate("ContentPlaceHolder1", new CompiledTemplateBuilder(BuildControlContent2));
            AddParsedSubObject(new LiteralControl("\r\n"));
            AddParsedSubObject(new LiteralControl("\r\n"));
        }

        protected override void FrameworkInitialize()
        {
            base.FrameworkInitialize();
            this.BuildControlTree();
            base.AddWrappedFileDependencies(_fileDependencies);
            base.Request.ValidateInput();
        }

        public override int GetTypeHashCode()
        {
            return 0x89e991c;
        }
    }
}


