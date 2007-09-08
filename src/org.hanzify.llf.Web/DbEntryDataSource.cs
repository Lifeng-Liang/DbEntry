
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Security.Permissions;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.QuerySyntax;

namespace Lephone.Web
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DbEntryDataSource<T> : DataSourceControl, IExcuteableDataSource
    {
        private static readonly ObjectInfo ObjInfo = DbObjectHelper.GetObjectInfo(typeof(T));
        protected static readonly string KeyName = ObjInfo.KeyFields[0].Name;
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
            string se = arguments.SortExpression;
            if (!string.IsNullOrEmpty(se))
            {
                DefaultOrderBy = se;
                ResetOrderBy();
            }
            int PageIndex = (int)(arguments.StartRowIndex / arguments.MaximumRows);
            int TotalRowCount = arguments.TotalRowCount;
            List<T> ret = ExecuteSelect(_Condition, m_OrderBy, arguments.MaximumRows, PageIndex, ref TotalRowCount);
            arguments.TotalRowCount = TotalRowCount;
            return ret;
        }

        public virtual List<T> ExecuteSelect(WhereCondition condition, OrderBy order, int MaximumRows, int PageIndex, ref int TotalRowCount)
        {
            IGetPagedSelector igp = DbEntry
                .From<T>()
                .Where(condition)
                .OrderBy(order)
                .PageSize(MaximumRows);
            IPagedSelector ps = _IsStatic ? igp.GetStaticPagedSelector() : igp.GetPagedSelector();
            TotalRowCount = (int)ps.GetResultCount();
            return (List<T>)ps.GetCurrentPage(PageIndex);
        }

        protected void ResetOrderBy()
        {
            Dictionary<string, string> odic = new Dictionary<string, string>();
            foreach (MemberHandler m in ObjInfo.SimpleFields)
            {
                if (m.MemberInfo.IsProperty)
                {
                    odic[m.MemberInfo.Name] = m.Name;
                }
                else
                {
                    odic[m.Name] = m.Name;
                }
            }
            List<ASC> las = new List<ASC>();
            foreach (ASC a in m_OrderBy.OrderItems)
            {
                string s = odic[a.Key];
                las.Add((a is DESC) ? new DESC(s) : new ASC(s));
            }
            m_OrderBy = new OrderBy(las.ToArray());
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
