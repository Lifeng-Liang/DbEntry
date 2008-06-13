using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
	public class LephoneEnum : DbObject
	{
		public int Type;

        [Length(1, 50)]
        public string Name;

        public int? Value;

        public LephoneEnum() {}

        public LephoneEnum(int Type, string Name, int? Value)
        {
            this.Type = Type;
            this.Name = Name;
            this.Value = Value;
        }
	}
}
