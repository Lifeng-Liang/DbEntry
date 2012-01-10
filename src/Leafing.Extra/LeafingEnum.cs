using Leafing.Data.Definition;

namespace Leafing.Extra
{
	public class LeafingEnum : DbObject
	{
		public int Type;

        [Length(1, 50)]
        public string Name;

        public int? Value;

        public LeafingEnum() {}

        public LeafingEnum(int type, string name, int? value)
        {
            this.Type = type;
            this.Name = name;
            this.Value = value;
        }
	}
}
