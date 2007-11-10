
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Security.Permissions;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.QuerySyntax;
using Lephone.Web.Common;
using Lephone.Util;
using Lephone.Data.Definition;

namespace Lephone.Web
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract partial class DbEntryDataSource<T> : DataSourceControl, IExcuteableDataSource where T : IDbObject
    {
        private static readonly ObjectInfo ObjInfo = ObjectInfo.GetInstance(typeof(T));
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

        [Themeable(false), DefaultValue("Id DESC"), Category("Behavior")]
        public string DefaultOrderBy
        {
            get
            {
                object o = this.ViewState["DefaultOrderBy"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Id DESC";
            }
            set
            {
                this.ViewState["DefaultOrderBy"] = value;
            }
        }

        [Themeable(false), DefaultValue(false), Category("Behavior")]
        public bool IsStatic
        {
            get
            {
                object o = this.ViewState["IsStatic"];
                if (o != null)
                {
                    return (bool)o;
                }
                return false;
            }
            set
            {
                this.ViewState["IsStatic"] = value;
            }
        }

        private WhereCondition _Condition;

        [Browsable(false)]
        public WhereCondition Condition
        {
            get { return _Condition; }
            set { _Condition = value; }
        }

        public DbEntryDataSource()
        {
        }

        IEnumerable IExcuteableDataSource.Select(DataSourceSelectArguments arguments)
        {
            m_OrderBy = string.IsNullOrEmpty(DefaultOrderBy) ? new OrderBy((DESC)"Id") : OrderBy.Parse(DefaultOrderBy);

            arguments.AddSupportedCapabilities(DataSourceCapabilities.Sort);
            arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);
            arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
            string se = arguments.SortExpression;
            if (!string.IsNullOrEmpty(se))
            {
                DefaultOrderBy = se;
                m_OrderBy = OrderBy.Parse(se);
            }
            int PageIndex = (arguments.MaximumRows == 0) ? 0 : (int)(arguments.StartRowIndex / arguments.MaximumRows);
            int TotalRowCount = arguments.TotalRowCount;
            List<T> ret = ExecuteSelect(_Condition, m_OrderBy, arguments.MaximumRows, PageIndex, ref TotalRowCount);
            arguments.TotalRowCount = TotalRowCount;
            return ret;
        }

        public virtual List<T> ExecuteSelect(WhereCondition condition, OrderBy order, int MaximumRows, int PageIndex, ref int TotalRowCount)
        {
            if (MaximumRows == 0)
            {
                return DbEntry.From<T>().Where(condition).OrderBy(order).Select();
            }
            else
            {
                IGetPagedSelector igp = DbEntry
                    .From<T>()
                    .Where(condition)
                    .OrderBy(order)
                    .PageSize(MaximumRows);
                IPagedSelector ps = IsStatic ? igp.GetStaticPagedSelector() : igp.GetPagedSelector();
                TotalRowCount = (int)ps.GetResultCount();
                IList result = ps.GetCurrentPage(PageIndex);
                return (List<T>)result;
            }
        }

        int IExcuteableDataSource.Delete(IDictionary keys, IDictionary values)
        {
            object key = Convert.ChangeType(keys[KeyName], ObjInfo.KeyFields[0].FieldType);
            T obj = DbEntry.GetObject<T>(key);
            int n = ExecuteDelete(obj);
            if (OnObjectDeleted != null)
            {
                OnObjectDeleted(obj);
            }
            return n;
        }

        public virtual int ExecuteDelete(object obj)
        {
            if (obj != null)
            {
                if (obj is DbObjectSmartUpdate)
                {
                    ((DbObjectSmartUpdate)obj).Delete();
                }
                else
                {
                    DbEntry.Delete(obj);
                }
                return 1;
            }
            return 0;
        }

        int IExcuteableDataSource.Insert(IDictionary values)
        {
            T obj = CreateObject(null, values);
            int n = ExecuteInsert(obj);
            if (OnObjectInserted != null)
            {
                OnObjectInserted(obj);
            }
            return n;
        }

        public virtual int ExecuteInsert(object obj)
        {
            if (obj != null)
            {
                if (obj is DbObjectSmartUpdate)
                {
                    ((DbObjectSmartUpdate)obj).Save();
                }
                else
                {
                    DbEntry.Save(obj);
                }
                return 1;
            }
            return 0;
        }

        int IExcuteableDataSource.Update(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            T obj = CreateObject(keys, values);
            int n = ExecuteUpdate(obj);
            if (OnObjectUpdated != null)
            {
                OnObjectUpdated(obj);
            }
            return n;
        }

        public virtual int ExecuteUpdate(object obj)
        {
            if (obj != null)
            {
                if (obj is DbObjectSmartUpdate)
                {
                    ((DbObjectSmartUpdate)obj).Save();
                }
                else
                {
                    DbEntry.Save(obj);
                }
                return 1;
            }
            return 0;
        }

        protected virtual T CreateObject(IDictionary keys, IDictionary values)
        {
            T obj;
            object key = null;
            if (keys != null)
            {
                key = Convert.ChangeType(keys[KeyName], ObjInfo.KeyFields[0].FieldType);
            }
            if (key == null || key.Equals(ObjInfo.KeyFields[0].UnsavedValue))
            {
                obj = (T)ObjInfo.NewObject();
            }
            else
            {
                obj = DbEntry.GetObject<T>(key);
            }
            foreach (MemberHandler mh in ObjInfo.SimpleFields)
            {
                string name = mh.MemberInfo.IsProperty ? mh.MemberInfo.Name : mh.Name;
                if (name != KeyName)
                {
                    if (values.Contains(name))
                    {
                        object ov = values[name];
                        object mo;
                        if (ov != null)
                        {
                            mo = Convert.ChangeType(ov.ToString(), mh.FieldType);
                        }
                        else
                        {
                            if (!mh.AllowNull)
                            {
                                mo = "";
                            }
                            else
                            {
                                mo = null;
                            }
                        }
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

            public override bool CanUpdate { get { return true; } }

            protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
            {
                return owner.Update(keys, values, oldValues);
            }
        }
    }
}
