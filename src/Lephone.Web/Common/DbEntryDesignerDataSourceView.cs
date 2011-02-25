using System;
using System.Collections.Generic;
using System.Web.UI.Design;
using System.Web.UI;
using Lephone.Core;
using Lephone.Data.Model;
using Lephone.Data.Model.Member;

namespace Lephone.Web.Common
{
    public class DbEntryDesignerDataSourceView : DesignerDataSourceView
    {
        private readonly DataSourceControl _dataSource;
        private readonly Type _modelType;
        private readonly ObjectInfo _info;

        public DbEntryDesignerDataSourceView(DbEntryDataSourceDesigner owner)
            : base(owner, "MainView")
        {
            _dataSource = (DataSourceControl)owner.Component;
            _modelType = _dataSource.GetType().BaseType.GetGenericArguments()[0];
            _info = ObjectInfoFactory.Instance.GetInstance(_modelType);
        }

        #region Visiable

        public override bool CanDelete
        {
            get { return true; }
        }

        public override bool CanInsert
        {
            get { return true; }
        }

        public override bool CanPage
        {
            get { return true; }
        }

        public override bool CanRetrieveTotalRowCount
        {
            get { return true; }
        }

        public override bool CanSort
        {
            get { return true; }
        }

        public override bool CanUpdate
        {
            get { return true; }
        }

        #endregion

        public override System.Collections.IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData)
        {
            isSampleData = true;

            var list = new List<object>();

            for (int i = 0; i < minimumRows; i++)
            {
                object obj = ClassHelper.CreateInstance(_info.HandleType);
                foreach (MemberHandler mh in _info.SimpleMembers)
                {
                    object value = GetSampleValue(mh.MemberType, mh.Is.AllowNull, i);
                    mh.SetValue(obj, value);
                }
                list.Add(obj);
            }

            return list;
        }

        private static object GetSampleValue(Type fieldType, bool allowNull, int n)
        {
            if (fieldType == typeof(string))
            {
                return "Sample Data " + n;
            }
            if (allowNull)
            {
                return GetSampleValue(fieldType.GetGenericArguments()[0], false, n);
            }
            if (fieldType == typeof(int))
            {
                return (int)(1 + n);
            }
            if (fieldType == typeof(short))
            {
                return (short)(1 + n);
            }
            if (fieldType == typeof(long))
            {
                return (long)(1 + n);
            }
            if (fieldType == typeof(float))
            {
                return (float)(0.1 + n);
            }
            if (fieldType == typeof(double))
            {
                return (double)(0.1 + n);
            }
            if (fieldType == typeof(bool))
            {
                return (n % 2) == 0;
            }
            if (fieldType == typeof(DateTime))
            {
                var dt = new DateTime(2008, 3, 6, 12, 24, 35);
                return dt.AddDays(n);
            }
            if (fieldType == typeof(Date))
            {
                var d = new Date(2008, 3, 17);
                return d.AddDays(n);
            }
            if (fieldType == typeof(Time))
            {
                var t = new Time(12, 24, 35);
                return t.AddMinutes(n);
            }
            if (fieldType == typeof(Guid))
            {
                return Guid.NewGuid();
            }
            if (fieldType == typeof(byte))
            {
                return (byte)n;
            }
            if (fieldType == typeof(sbyte))
            {
                return (sbyte)n;
            }
            if (fieldType == typeof(decimal))
            {
                return (decimal)n;
            }
            if (fieldType == typeof(byte[]))
            {
                return new byte[] { 61, 62, 63 };
            }
            if (fieldType.IsEnum)
            {
                string[] ss = Enum.GetNames(fieldType);
                string name = ss[n % ss.Length];
                return Enum.Parse(fieldType, name);
            }
            return null;
        }

        public override IDataSourceViewSchema Schema
        {
            get
            {
                return new DbEntryDataSourceViewSchema(_info);
            }
        }
    }
}
