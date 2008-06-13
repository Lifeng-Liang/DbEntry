using System;
using Lephone.Util.Setting;

namespace Lephone.Util.TimingTask
{
    public class NowProvider
    {
        public static readonly NowProvider Instance = (NowProvider)ClassHelper.CreateInstance(UtilSetting.NowProvider);

        protected NowProvider() {}

        public virtual DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}