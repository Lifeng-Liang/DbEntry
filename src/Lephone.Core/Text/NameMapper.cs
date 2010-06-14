using Lephone.Core.Setting;

namespace Lephone.Core.Text
{
    public class NameMapper
    {
        public static readonly NameMapper Instance = (NameMapper)ClassHelper.CreateInstance(CoreSettings.NameMapper);

        public virtual string MapName(string name)
        {
            return name;
        }

        public virtual string UnmapName(string name)
        {
            return name;
        }

        public virtual string Prefix
        {
            get { return string.Empty; }
        }
    }
}
