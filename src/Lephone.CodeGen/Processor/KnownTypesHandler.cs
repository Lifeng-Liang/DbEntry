using Lephone.Data.Definition;
using Lephone.Util;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class KnownTypesHandler
    {
        public readonly MethodReference UpdateColumn;

        public KnownTypesHandler(ModuleDefinition module)
        {
            UpdateColumn = module.Import(typeof(DbObjectSmartUpdate).GetMethod("m_ColumnUpdated", ClassHelper.InstanceFlag));
        }
    }
}
