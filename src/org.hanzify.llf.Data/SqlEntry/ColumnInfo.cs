
#region usings

using System;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
{
    public class ColumnInfo : KeyValue
    {
        public bool IsKey;
        public bool IsDbGenerate;
        public bool AllowNull;
        public bool IsUnicode;
        public int Length;

        public ColumnInfo(string Key, Type ValueType, bool IsKey, bool IsDbGenerate, bool AllowNull, bool IsUnicode, int Length)
            : base(Key, null, ValueType)
        {
            this.IsKey = IsKey;
            this.IsDbGenerate = IsDbGenerate;
            this.AllowNull = AllowNull;
            this.IsUnicode = IsUnicode;
            this.Length = Length;
        }

        internal ColumnInfo(MemberHandler fh)
            : this(fh.Name, GetType(fh), fh.IsKey, fh.IsDbGenerate, fh.AllowNull, fh.IsUnicode, fh.MaxLength)
        {
        }

        private static Type GetType(MemberHandler fh)
        {
            if (fh.IsBelongsTo)
            {
                Common.ObjectInfo oi = Common.DbObjectHelper.GetObjectInfo(fh.FieldType.GetGenericArguments()[0]);
                if (oi.KeyFields != null && oi.KeyFields.Length == 1)
                {
                    return oi.KeyFields[0].FieldType;
                }
            }
            else if(fh.IsLazyLoad)
            {
                return fh.FieldType.GetGenericArguments()[0];
            }
            return fh.FieldType;
        }
    }
}
