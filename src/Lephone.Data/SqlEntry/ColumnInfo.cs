using System;
using Lephone.Data.Model.Member;

namespace Lephone.Data.SqlEntry
{
    public class ColumnInfo : KeyValue
    {
        public bool IsKey;
        public bool IsDbGenerate;
        public bool AllowNull;
        public bool IsUnicode;
        public int Length;
        public int DecimalPart;

        public ColumnInfo(string key, Type valueType, bool isKey, bool isDbGenerate, bool allowNull, bool isUnicode, int length, int decimalPart)
            : base(key, null, valueType)
        {
            this.IsKey = isKey;
            this.IsDbGenerate = isDbGenerate;
            this.AllowNull = allowNull;
            this.IsUnicode = isUnicode;
            this.Length = length;
            this.DecimalPart = decimalPart;
        }

        internal ColumnInfo(MemberHandler fh)
            : this(fh.Name, GetType(fh), fh.Is.Key, fh.Is.DbGenerate, fh.Is.AllowNull, fh.Is.Unicode, fh.MaxLength, fh.DecimalPart)
        {
        }

        private static Type GetType(MemberHandler fh)
        {
            if (fh.Is.BelongsTo)
            {
                var ctx = ModelContext.GetInstance(fh.FieldType.GetGenericArguments()[0]);
                if (ctx.Info.KeyMembers != null && ctx.Info.KeyMembers.Length == 1)
                {
                    return ctx.Info.KeyMembers[0].FieldType;
                }
            }
            else if(fh.Is.LazyLoad)
            {
                return fh.FieldType.GetGenericArguments()[0];
            }
            return fh.FieldType;
        }
    }
}
