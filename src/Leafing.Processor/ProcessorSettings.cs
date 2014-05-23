using Leafing.Core;
using Mono.Cecil.Cil;

namespace Leafing.Processor
{
    public static class ProcessorSettings
    {
        public static readonly string SymbolReaderProvider = "Mono.Cecil.Pdb.PdbReaderProvider, Mono.Cecil.Pdb";
        public static readonly string SymbolWriterProvider = "Mono.Cecil.Pdb.PdbWriterProvider, Mono.Cecil.Pdb";
        public static readonly bool AddCompareToSetProperty = true;

        public static ISymbolReaderProvider GetSymbolReaderProvider()
        {
            return (ISymbolReaderProvider)ClassHelper.CreateInstance(SymbolReaderProvider);
        }

        public static ISymbolWriterProvider GetSymbolWriterProvider()
        {
            return (ISymbolWriterProvider)ClassHelper.CreateInstance(SymbolWriterProvider);
        }

        public static string GetExtName()
        {
            if(SymbolReaderProvider.EndsWith(".Pdb"))
            {
                return ".pdb";
            }
            return ".mdb";
        }

        static ProcessorSettings()
        {
            typeof(ProcessorSettings).Initialize();
        }
    }
}
