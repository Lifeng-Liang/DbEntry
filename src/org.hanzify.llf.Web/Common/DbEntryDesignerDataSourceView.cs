using System;
using System.Collections.Generic;
using System.Web.UI.Design;
using System.Web.UI;
using Lephone.Data.Common;

namespace Lephone.Web.Common
{
    public class DbEntryDesignerDataSourceView : DesignerDataSourceView
    {
        private readonly DataSourceControl DataSource;
        private readonly Type ModelType;
        private readonly ObjectInfo oi;

        public DbEntryDesignerDataSourceView(DbEntryDataSourceDesigner owner)
            : base(owner, "MainView")
        {
            DataSource = (DataSourceControl)owner.Component;
            ModelType = DataSource.GetType().BaseType.GetGenericArguments()[0];
            oi = ObjectInfo.GetInstance(ModelType);
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

            List<object> list = new List<object>();

            for (int i = 0; i < minimumRows; i++)
            {
                object obj = oi.NewObject();
                foreach (MemberHandler mh in oi.SimpleFields)
                {
                    object value = GetSampleValue(mh.FieldType, mh.AllowNull, i);
                    mh.SetValue(obj, value);
                }
                list.Add(obj);
            }

            return list;
        }

        private object GetSampleValue(Type FieldType, bool AllowNull, int n)
        {
            if (FieldType == typeof(string))
            {
                return "Sample Data " + n;
            }
            if (AllowNull)
            {
                return GetSampleValue(FieldType.GetGenericArguments()[0], false, n);
            }
            if (FieldType == typeof(int))
            {
                return (int)(1 + n);
            }
            if (FieldType == typeof(short))
            {
                return (short)(1 + n);
            }
            if (FieldType == typeof(long))
            {
                return (long)(1 + n);
            }
            if (FieldType == typeof(float))
            {
                return (float)(0.1 + n);
            }
            if (FieldType == typeof(double))
            {
                return (double)(0.1 + n);
            }
            if (FieldType == typeof(bool))
            {
                return (n % 2) == 0;
            }
            if (FieldType == typeof(DateTime))
            {
                DateTime dt = new DateTime(2008, 3, 6, 12, 24, 35);
                return dt.AddDays(n);
            }
            if (FieldType == typeof(Date))
            {
                Date d = new Date(2008, 3, 17);
                return d.AddDays(n);
            }
            if (FieldType == typeof(Time))
            {
                Time t = new Time(12, 24, 35);
                return t.AddMinutes(n);
            }
            if (FieldType == typeof(Guid))
            {
                return Guid.NewGuid();
            }
            if (FieldType == typeof(byte))
            {
                return (byte)n;
            }
            if (FieldType == typeof(sbyte))
            {
                return (sbyte)n;
            }
            if (FieldType == typeof(decimal))
            {
                return (decimal)n;
            }
            if (FieldType == typeof(byte[]))
            {
                return new byte[] { 61, 62, 63 };
            }
            if (FieldType.IsEnum)
            {
                string[] ss = Enum.GetNames(FieldType);
                string name = ss[n % ss.Length];
                return Enum.Parse(FieldType, name);
            }
            return null;
        }

        public override IDataSourceViewSchema Schema
        {
            get
            {
                return new DbEntryDataSourceViewSchema(oi);
            }
        }
    }
}
