using System;
using System.Web.UI.Design;

namespace Lephone.Web.Common
{
    public class DbEntryDataSourceDesigner : DataSourceDesigner
    {
        public override bool CanConfigure
        {
            get { return false; }
        }

        public override bool CanRefreshSchema
        {
            get { return false; }
        }

        public override void Configure()
        {
            throw new NotImplementedException();
        }

        public override DesignerDataSourceView GetView(string viewName)
        {
            return new DbEntryDesignerDataSourceView(this);
        }

        public override string[] GetViewNames()
        {
            return new string[] { "MainView" };
        }

        public override void RefreshSchema(bool preferSilent)
        {
            throw new NotImplementedException();
        }
    }
}
