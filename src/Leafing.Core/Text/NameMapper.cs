using Leafing.Core.Ioc;
using Leafing.Core.Setting;

namespace Leafing.Core.Text
{
    [DependenceEntry, Implementation("Default")]
    public class NameMapper
    {
        public static readonly NameMapper Instance;

        static NameMapper()
        {
            if(CoreSettings.NameMapper.StartsWith("@"))
            {
                Instance = SimpleContainer.Get<NameMapper>(CoreSettings.NameMapper.Substring(1));
            }
            else
            {
                Instance = (NameMapper)ClassHelper.CreateInstance(CoreSettings.NameMapper);
            }
        }

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
