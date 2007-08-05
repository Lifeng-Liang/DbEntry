
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Security.Permissions;

namespace org.hanzify.llf.Data
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DbEntryDataSource<T> : DataSourceControl
    {
        public event EventHandler DataSourceChanged;

        private DbEntryDataSourceView<T> view = null;

        protected override DataSourceView GetView(string viewName)
        {
            if (view == null)
            {
                view = new DbEntryDataSourceView<T>(this, viewName);
            }
            return view;
        }

        protected override ICollection GetViewNames()
        {
            return new string[] { "MainView" };
        }

        public class DbEntryDataSourceView<T1> : DataSourceView
        {
            public DbEntryDataSourceView(IDataSource owner, string viewName)
                : base(owner, viewName)
            {
            }

            protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
            {
                //System.Diagnostics.Debug.Print("{0}, {1}", arguments.StartRowIndex, arguments.MaximumRows);
                return DbEntry
                    .From<T1>()
                    .Where(null)
                    .OrderBy((DESC)"Id")
                    //.Range(arguments.StartRowIndex, arguments.StartRowIndex + arguments.MaximumRows)
                    .Select();
            }

            public override bool CanPage { get { return true; } }

            public override bool CanDelete { get { return false; } }

            protected override int ExecuteDelete(IDictionary keys, IDictionary values)
            {
                throw new NotSupportedException();
            }

            public override bool CanInsert { get { return false; } }

            protected override int ExecuteInsert(IDictionary values)
            {
                throw new NotSupportedException();
            }

            public override bool CanUpdate { get { return false; } }

            protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
            {
                throw new NotSupportedException();
            }
        }
    }
}
