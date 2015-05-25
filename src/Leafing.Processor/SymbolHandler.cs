using System;
using Mono.Cecil.Cil;
using System.IO;
using Leafing.Core;

namespace Leafing.Processor
{
	public class SymbolHandler
	{
		public bool HasSymbols { get; private set; }
		public ISymbolReaderProvider Reader { get; private set; }
		public ISymbolWriterProvider Writer { get; private set; }

		public SymbolHandler (string fileName)
		{
			if (File.Exists (fileName + ".pdb")) {
				Init ("Mono.Cecil.Pdb.PdbReaderProvider, Mono.Cecil.Pdb",
					"Mono.Cecil.Pdb.PdbWriterProvider, Mono.Cecil.Pdb");
			} else if (File.Exists (fileName + ".mdb")) {
				Init ("Mono.Cecil.Mdb.PdbReaderProvider, Mono.Cecil.Mdb",
					"Mono.Cecil.Mdb.PdbWriterProvider, Mono.Cecil.Mdb");
			} else {
				HasSymbols = false;
			}
		}

		private void Init(string readerName, string writerName)
		{
			HasSymbols = true;
			Reader = (ISymbolReaderProvider)ClassHelper.CreateInstance(readerName);
			Writer = (ISymbolWriterProvider)ClassHelper.CreateInstance(writerName);
		}
	}
}

