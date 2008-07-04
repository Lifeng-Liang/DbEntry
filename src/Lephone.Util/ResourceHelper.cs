using System;
using System.Reflection;
using System.IO;

namespace Lephone.Util
{
	public static class ResourceHelper
	{
		private static readonly Assembly assembly = Assembly.GetEntryAssembly();

		#region GetStream

        public static Stream GetStream(Assembly Asm, string FilePath, bool AddPrefix)
		{
            string fn = (AddPrefix) ? string.Format("{0}.{1}", Asm.GetName().Name, FilePath) : FilePath;
			return Asm.GetManifestResourceStream(fn);
		}

        public static Stream GetStream(Assembly Asm, string FilePath)
        {
            return GetStream(Asm, FilePath, true);
        }

        public static Stream GetStream(Type t, string FilePath)
		{
			Assembly Asm = Assembly.GetAssembly(t);
			return GetStream(Asm, FilePath);
		}

		public static Stream GetStream(string FilePath)
		{
			string fn = string.Format("{0}.{1}",  assembly.GetName().Name, FilePath);
			return assembly.GetManifestResourceStream(fn);
		}

		#endregion

		#region GetStreamReader

        public static StreamReader GetStreamReader(Assembly Asm, string FilePath, bool AddPrefix)
        {
            return new StreamReader(GetStream(Asm, FilePath, AddPrefix));
        }

        public static StreamReader GetStreamReader(Assembly Asm, string FilePath)
		{
			return new StreamReader(GetStream(Asm, FilePath));
		}

		public static StreamReader GetStreamReader(Type t, string FilePath)
		{
			Assembly Asm = Assembly.GetAssembly(t);
			return GetStreamReader(Asm, FilePath);
		}

		public static StreamReader GetStreamReader(string FilePath)
		{
			return GetStreamReader(assembly, FilePath);
		}

		#endregion

		#region ReadToEnd

		public static string ReadToEnd(Assembly Asm, string FilePath, bool AddPrefix)
		{
            using (StreamReader sr = GetStreamReader(Asm, FilePath, AddPrefix))
			{
				return sr.ReadToEnd();
			}
		}

        public static string ReadToEnd(Assembly Asm, string FilePath)
        {
            return ReadToEnd(Asm, FilePath, true);
        }

        public static string ReadToEnd(Type t, string FilePath)
		{
			Assembly Asm = Assembly.GetAssembly(t);
			return ReadToEnd(Asm, FilePath);
		}

		public static string ReadToEnd(string FilePath)
		{
			return ReadToEnd(assembly, FilePath);
		}

        public static byte[] ReadAll(Type t, string FilePath)
        {
            using (Stream s = GetStream(t, FilePath))
            {
                int l = (int)s.Length;
                byte[] ret = new byte[l];
                s.Read(ret, 0, l);
                return ret;
            }
        }

        #endregion
	}
}
