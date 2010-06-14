using System;
using System.Data;
using System.Reflection;
using Lephone.Core;

namespace Lephone.Data.SqlEntry
{
    public class DbColumnInfo
    {
        public string ColumnName;
        public int ColumnOrdinal;
        public int ColumnSize;
        public short NumericPrecision;
        public short NumericScale;
        public bool IsUnique;
        public bool IsKey;
        public string BaseCatalogName;
        public string BaseColumnName;
        public string BaseSchemaName;
        public string BaseTableName;
        public Type DataType;
        public bool AllowDBNull;
        public int ProviderType;
        public bool IsAutoIncrement;
        public bool IsRowVersion;
        public bool IsLong;
        public bool IsReadOnly;

        public DbColumnInfo(DataRow dr)
        {
            Type t = typeof(DbColumnInfo);
            var fis = t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo fi in fis)
            {
                CommonHelper.CatchAll(
                    delegate
                    {
                        object o = dr[fi.Name];
                        if (o == DBNull.Value)
                        {
                            o = null;
                        }
                        if (o != null && fi.FieldType != typeof(Type) && o.GetType() != fi.FieldType)
                        {
                            o = ClassHelper.ChangeType(o, fi.FieldType);
                        }
                        fi.SetValue(this, o);
                    });
            }
        }
    }
}
