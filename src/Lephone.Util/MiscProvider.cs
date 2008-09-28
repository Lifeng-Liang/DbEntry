using System;
using Lephone.Util.Setting;

namespace Lephone.Util
{
    public class MiscProvider
    {
        public static readonly MiscProvider Instance = (MiscProvider)ClassHelper.CreateInstance(UtilSetting.MiscProvider);

        protected MiscProvider() {}

        public virtual DateTime Now
        {
            get { return DateTime.Now; }
        }

        public virtual Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}