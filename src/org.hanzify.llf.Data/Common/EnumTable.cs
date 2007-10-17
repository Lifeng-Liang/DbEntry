
#region usings

using System;
using Lephone.Data.Definition;

#endregion

namespace Lephone.Data.Common
{
    [DisableSqlLog, DbTable("Lephone_Enum")]
	public class EnumTable : DbObject
	{
		public int Type;

        [Length(1, 50)]
        public string Name;

        [DbColumn("Value")]
        protected internal int? m_Value;

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
