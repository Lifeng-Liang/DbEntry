using Lephone.Data.Definition;

namespace Lephone.Utility
{
	public class LephoneEnum : DbObject
	{
		public int Type;

        [Length(1, 50)]
        public string Name;

        public int? Value;

        public LephoneEnum() {}

        public LephoneEnum(int type, string name, int? value)
        {
            this.Type = type;
            this.Name = name;
            this.Value = value;
        }
	}
}
