using Leafing.Core;
using Leafing.Core.Setting;
using Mono.Cecil.Cil;

namespace Leafing.Processor
{
    public static class ProcessorSettings
    {
        public static readonly bool AddCompareToSetProperty = true;

        static ProcessorSettings()
        {
            ConfigHelper.LeafingSettings.InitClass(typeof(ProcessorSettings));
        }
    }
}
