
#region usings

using System;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.Data.Common
{
    [DisableSqlLog]
	public class EnumTable : DbObject
	{
		public int Type;

        [MaxLength(50)]
        public string Name;

        [DbColumn("Value")]
        protected int? m_Value;

        [Exclude]
        public int? Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public EnumTable() {}

        public EnumTable(int Type, string Name, int? Value)
        {
            this.Type = Type;
            this.Name = Name;
            this.Value = Value;
        }
	}
}
