using Leafing.Core.Ioc;
using Leafing.Core.Setting;

namespace Leafing.Core.Text {
    [DependenceEntry, Implementation("Default")]
    public class NameMapper {
        public static readonly NameMapper Instance;

        static NameMapper() {
            var nameMapper = ConfigReader.Config.Database.NameMapper;
            if (nameMapper.StartsWith("@")) {
                Instance = SimpleContainer.Get<NameMapper>(nameMapper.Substring(1));
            } else {
                Instance = (NameMapper)ClassHelper.CreateInstance(nameMapper);
            }
        }

        public virtual string MapName(string name) {
            return name;
        }

        public virtual string UnmapName(string name) {
            return name;
        }

        public virtual string Prefix {
            get { return string.Empty; }
        }
    }
}
