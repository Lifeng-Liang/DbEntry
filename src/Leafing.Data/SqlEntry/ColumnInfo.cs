using System;
using Leafing.Data.Model.Member;

namespace Leafing.Data.SqlEntry
{
    public class ColumnInfo : KeyValue
    {
        public readonly bool IsKey;
        public readonly bool IsForeignKey;
        public readonly bool IsDbGenerate;
        public readonly bool AllowNull;
        public readonly bool IsUnicode;
        public readonly int Length;
        public readonly int DecimalPart;
        public readonly string BelongsToTableName;

        public ColumnInfo(string key, Type valueType, string btName)
            : this(key, valueType, false, false, false, false, 0, 0, btName)
        {
        }

        public ColumnInfo(string key, Type valueType, bool isKey, bool isDbGenerate, bool allowNull, bool isUnicode, int length, int decimalPart, string btName)
            : base(key, null, valueType)
        {
            this.IsKey = isKey;
            this.IsForeignKey = !btName.IsNullOrEmpty();
            this.IsDbGenerate = isDbGenerate;
            this.AllowNull = allowNull;
            this.IsUnicode = isUnicode;
            this.Length = length;
            this.DecimalPart = decimalPart;
            this.BelongsToTableName = btName;
        }

        internal ColumnInfo(MemberHandler mh)
            : this(mh.Name, GetType(mh), mh.Is.Key, mh.Is.DbGenerate, mh.Is.AllowNull, mh.Is.Unicode, mh.MaxLength, mh.DecimalPart, GetBtName(mh))
        {
        }

        private static string GetBtName(MemberHandler mh)
        {
            if(mh.Is.BelongsTo)
            {
                var t = mh.MemberType.GetGenericArguments()[0];
                var ctx = ModelContext.GetInstance(t);
                return ctx.Info.From.MainTableName;
            }
            return null;
        }

        private static Type GetType(MemberHandler fh)
        {
            if (fh.Is.BelongsTo)
            {
                var ctx = ModelContext.GetInstance(fh.MemberType.GetGenericArguments()[0]);
                if (ctx.Info.KeyMembers != null && ctx.Info.KeyMembers.Length == 1)
                {
                    return ctx.Info.KeyMembers[0].MemberType;
                }
            }
            else if(fh.Is.LazyLoad)
            {
                return fh.MemberType.GetGenericArguments()[0];
            }
            return fh.MemberType;
        }
    }
}
