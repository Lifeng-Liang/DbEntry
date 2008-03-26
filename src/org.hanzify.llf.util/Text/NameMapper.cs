
using System;
using Lephone.Util.Setting;

namespace Lephone.Util.Text
{
    public class NameMapper
    {
        public static readonly NameMapper Instance = (NameMapper)ClassHelper.CreateInstance(UtilSetting.NameMapper);

        public virtual string MapName(string Name)
        {
            return Name;
        }

        public virtual string UnmapName(string Name)
        {
            return Name;
        }

        public virtual string Prefix
        {
            get { return string.Empty; }
        }
    }
}
