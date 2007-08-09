
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Security.Permissions;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.QuerySyntax;

namespace org.hanzify.llf.Data
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DbEntryDataSource<T> : DataSourceControl, IExcuteableDataSource
    {
        protected static readonly string KeyName = DbObjectHelper.GetObjectInfo(typeof(T)).KeyFields[0].Name;
        public event EventHandler DataSourceChanged;

        protected void RaiseDataSourceChanged()
        {
            if (DataSourceChanged != null)
            {
                DataSourceChanged(this, new EventArgs());
            }
        }

        private DbEntryDataSourceView view = null;

        protected override DataSourceView GetView(string viewName)
        {
            if (view == null)
            {
                view = new DbEntryDataSourceView(this, viewName);
            }
            return view;
        }

        protected override ICollection GetViewNames()
        {
            return new string[] { "MainView" };
        }

        private OrderBy m_OrderBy;
        private string _DefaultOrderBy;

        [Browsable(true), EditorBrowsable]
        public string DefaultOrderBy
        {
            get { return _DefaultOrderBy; }
            set
            {
                m_OrderBy = string.IsNullOrEmpty(value) ? new OrderBy((DESC)"Id") : OrderBy.Parse(value);
                _DefaultOrderBy = value;
            }
        }

        private bool _IsStatic = false;

        [Browsable(true), EditorBrowsable]
        public bool IsStatic
        {
            get { return _IsStatic; }
            set { _IsStatic = value; }
        }

        private WhereCondition _Condition;

        public WhereCondition Condition
        {
            get { return _Condition; }
            set { _Condition = value; }
        }

        public DbEntryDataSource()
        {
            DefaultOrderBy = "";
        }

        IEnumerable IExcuteableDataSource.Select(DataSourceSelectArguments arguments)
        {
            arguments.AddSupportedCapabilities(DataSourceCapabilities.Sort);
            arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);
            arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
            return ExecuteSelect(arguments);
        }

        public virtual List<T> ExecuteSelect(DataSourceSelectArguments arguments)
        {
            string se = arguments.SortExpression;
            if (!string.IsNullOrEmpty(se))
            {
                DefaultOrderBy = se;
            }
            IGetPagedSelector igp = DbEntry
                .From<T>()
                .Where(_Condition)
                .OrderBy(m_OrderBy)
                .PageSize(arguments.MaximumRows);
            IPagedSelector ps = _IsStatic ? igp.GetStaticPagedSelector() : igp.GetPagedSelector();
            arguments.TotalRowCount = (int)ps.GetResultCount();
            int PageIndex = (int)(arguments.StartRowIndex / arguments.MaximumRows);
            return (List<T>)ps.GetCurrentPage(PageIndex);
        }

        int IExcuteableDataSource.Delete(IDictionary keys, IDictionary values)
        {
            return ExecuteDelete(values[KeyName]);
        }

        public virtual int ExecuteDelete(object Key)
        {
            return DbEntry.Context.Delete<T>(CK.K[KeyName] == Key);
        }

        int IExcuteableDataSource.Insert(IDictionary values)
        {
            T obj = CreateObject(values);
            return ExecuteInsert(obj);
        }

        public virtual int ExecuteInsert(object obj)
        {
            DbEntry.Save(obj);
            return 1;
        }

        int IExcuteableDataSource.Update(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            T obj = CreateObject(values);
            return ExecuteUpdate(obj);
        }

        public virtual int ExecuteUpdate(object obj)
        {
            DbEntry.Save(obj);
            return 1;
        }

        protected virtual T CreateObject(IDictionary values)
        {
            ObjectInfo ii = DbObjectHelper.GetObjectInfo(typeof(T));
            object key = Convert.ChangeType(values[KeyName], ii.KeyFields[0].FieldType);
            T obj;
            if (key.Equals(ii.KeyFields[0].UnsavedValue))
            {
                obj = (T)ii.NewObject();
            }
            else
            {
                obj = DbEntry.GetObject<T>(key);
            }
            foreach (MemberHandler mh in ii.SimpleFields)
            {
                string name = mh.MemberInfo.IsProperty ? mh.MemberInfo.Name : mh.Name;
                if (name != KeyName)
                {
                    if (values.Contains(name))
                    {
                        string ms = values[name].ToString();
                        object mo = Convert.ChangeType(ms, mh.FieldType);
                        if (!(mh.GetValue(obj).Equals(mo)))
                        {
                            mh.SetValue(obj, mo);
                        }
                    }
                }
            }
            return obj;
        }

        public class DbEntryDataSourceView : DataSourceView
        {
            IExcuteableDataSource owner;

            public DbEntryDataSourceView(IDataSource owner, string viewName)
                : base(owner, viewName)
            {
                this.owner = (IExcuteableDataSource)owner;
            }

            public override bool CanSort { get { return true; } }
            public override bool CanPage { get { return true; } }
            public override bool CanRetrieveTotalRowCount { get { return true; } }

            protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
            {
                return owner.Select(arguments);
            }

            public override bool CanDelete { get { return true; } }

            protected override int ExecuteDelete(IDictionary keys, IDictionary values)
            {
                return owner.Delete(keys, values);
            }

            public override bool CanInsert { get { return true; } }

            protected override int ExecuteInsert(IDictionary values)
            {
                return owner.Insert(values);
            }

            public override bool CanUpdate { get { return false; } }

            protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
            {
                return owner.Update(keys, values, oldValues);
            }
        }
    }
}
