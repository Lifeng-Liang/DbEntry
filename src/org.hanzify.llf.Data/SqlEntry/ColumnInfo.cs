
#region usings

using System;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
{
    public class ColumnInfo : KeyValue
    {
        public bool IsKey;
        public bool IsSystemGen;
        public bool AllowNull;
        public bool IsUnicode;
        public int Length;

        public ColumnInfo(string Key, Type ValueType, bool IsKey, bool IsSystemGen, bool AllowNull, bool IsUnicode, int Length)
            : base(Key, null, ValueType)
        {
            this.IsKey = IsKey;
            this.IsSystemGen = IsSystemGen;
            this.AllowNull = AllowNull;
            this.IsUnicode = IsUnicode;
            this.Length = Length;
        }

        internal ColumnInfo(MemberHandler fh)
            : this(fh.Name, fh.FieldType, fh.IsKey, fh.IsSystemGeneration, fh.AllowNull, fh.IsUnicode, fh.MaxLength)
        {
        }
    }
}
