using Lephone.Util.Setting;

namespace Lephone.Util.Text
{
    public class NameMapper
    {
        public static readonly NameMapper Instance = (NameMapper)ClassHelper.CreateInstance(UtilSetting.NameMapper);

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
