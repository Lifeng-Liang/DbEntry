using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Security.Permissions;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Linq;
using Lephone.Web.Common;
using Lephone.Core;
using Lephone.Data.Definition;

namespace Lephone.Web
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ParseChildren(true), PersistChildren(false)]
    [Designer(typeof(DbEntryDataSourceDesigner))]
    public abstract partial class DbEntryDataSource<T> : DataSourceControl, IExcuteableDataSource where T : class, IDbObject
    {
        public void AddAndCondition(Expression<Func<T, bool>> condition)
        {
            var c = ExpressionParser<T>.Parse(condition);
            Condition &= c;
        }

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

        private DbEntryDataSourceView _view;

        protected override DataSourceView GetView(string viewName)
        {
            return _view ?? (_view = new DbEntryDataSourceView(this, viewName));
        }

        protected override ICollection GetViewNames()
        {
            return new[] { "MainView" };
        }

        private OrderBy _mOrderBy;

        [Themeable(false), DefaultValue("Id DESC"), Category("Behavior")]
        public string DefaultOrderBy
        {
            get
            {
                object o = ViewState["DefaultOrderBy"];
                if (o != null)
                {
                    return (string)o;
                }
                return "Id DESC";
            }
            set
            {
                ViewState["DefaultOrderBy"] = value;
            }
        }

        [Themeable(false), DefaultValue(false), Category("Behavior")]
        public bool IsStatic
        {
            get
            {
                object o = ViewState["IsStatic"];
                if (o != null)
                {
                    return (bool)o;
                }
                return false;
            }
            set
            {
                ViewState["IsStatic"] = value;
            }
        }

        private Condition _condition;

        [Browsable(false)]
        public Condition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        IEnumerable IExcuteableDataSource.Select(DataSourceSelectArguments arguments)
        {
            _mOrderBy = string.IsNullOrEmpty(DefaultOrderBy) ? new OrderBy((DESC)"Id") : OrderBy.Parse(DefaultOrderBy);

            arguments.AddSupportedCapabilities(DataSourceCapabilities.Sort);
            arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);
            arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
            string se = arguments.SortExpression;
            if (!string.IsNullOrEmpty(se))
            {
                DefaultOrderBy = se;
                _mOrderBy = OrderBy.Parse(se, typeof(T));
            }
            int pageIndex = (arguments.MaximumRows == 0) ? 0 : arguments.StartRowIndex / arguments.MaximumRows;
            int totalRowCount = arguments.TotalRowCount;
            List<T> ret = ExecuteSelect(_condition, _mOrderBy, arguments.MaximumRows, pageIndex, ref totalRowCount);
            arguments.TotalRowCount = totalRowCount;
            return ret;
        }

        public virtual List<T> ExecuteSelect(Condition condition, OrderBy order, int maximumRows, int pageIndex, ref int totalRowCount)
        {
            if (maximumRows == 0)
            {
                return DbEntry.From<T>().Where(condition).OrderBy(order).Select();
            }
            var igp = DbEntry
                .From<T>()
                .Where(condition)
                .OrderBy(order)
                .PageSize(maximumRows);
            IPagedSelector ps = IsStatic ? igp.GetStaticPagedSelector() : igp.GetPagedSelector();
            totalRowCount = (int)ps.GetResultCount();
            IList result = ps.GetCurrentPage(pageIndex);
            return (List<T>)result;
        }

        int IExcuteableDataSource.Delete(IDictionary keys, IDictionary values)
        {
            object key = ClassHelper.ChangeType(keys[KeyName], ObjInfo.KeyFields[0].FieldType);
            var obj = DbEntry.GetObject<T>(key);
            int n = ExecuteDelete(obj);
            if (OnObjectDeleted != null)
            {
                OnObjectDeleted(obj);
            }
            return n;
        }

        public virtual int ExecuteDelete(T obj)
        {
            if (obj != null)
            {
                var o = obj as DbObjectSmartUpdate;
                if (o != null)
                {
                    o.Delete();
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

        public virtual int ExecuteInsert(T obj)
        {
            if (obj != null)
            {
                var o = obj as DbObjectSmartUpdate;
                if (o != null)
                {
                    o.Save();
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

        public virtual int ExecuteUpdate(T obj)
        {
            if (obj != null)
            {
                var o = obj as DbObjectSmartUpdate;
                if (o != null)
                {
                    o.Save();
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
                key = ClassHelper.ChangeType(keys[KeyName], ObjInfo.KeyFields[0].FieldType);
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
                            mo = ClassHelper.ChangeType(ov.ToString(), mh.FieldType);
                        }
                        else
                        {
                            mo = !mh.AllowNull ? "" : null;
                        }
                        object ho = mh.GetValue(obj);
                        if(!CommonHelper.AreEqual(ho, mo))
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
            readonly IExcuteableDataSource _owner;

            public DbEntryDataSourceView(IDataSource owner, string viewName)
                : base(owner, viewName)
            {
                this._owner = (IExcuteableDataSource)owner;
            }

            public override bool CanSort { get { return true; } }
            public override bool CanPage { get { return true; } }
            public override bool CanRetrieveTotalRowCount { get { return true; } }

            protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
            {
                return _owner.Select(arguments);
            }

            public override bool CanDelete { get { return true; } }

            protected override int ExecuteDelete(IDictionary keys, IDictionary values)
            {
                return _owner.Delete(keys, values);
            }

            public override bool CanInsert { get { return true; } }

            protected override int ExecuteInsert(IDictionary values)
            {
                return _owner.Insert(values);
            }

            public override bool CanUpdate { get { return true; } }

            protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
            {
                return _owner.Update(keys, values, oldValues);
            }
        }
    }
}
