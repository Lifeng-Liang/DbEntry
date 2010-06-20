using Lephone.Core;
using Lephone.Core.Setting;
using Mono.Cecil.Cil;

namespace Lephone.Processor
{
    public static class ProcesssorSettings
    {
        public static readonly string SymbolReaderProvider = "Mono.Cecil.Pdb.PdbReaderProvider, Mono.Cecil.Pdb";
        public static readonly string SymbolWriterProvider = "Mono.Cecil.Pdb.PdbWriterProvider, Mono.Cecil.Pdb";

        public static ISymbolReaderProvider GetSymbolReaderProvider()
        {
            return (ISymbolReaderProvider)ClassHelper.CreateInstance(SymbolReaderProvider);
        }

        public static ISymbolWriterProvider GetSymbolWriterProvider()
        {
            return (ISymbolWriterProvider)ClassHelper.CreateInstance(SymbolWriterProvider);
        }

        static ProcesssorSettings()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(ProcesssorSettings));
        }
    }
}
