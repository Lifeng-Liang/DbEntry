using System;
using System.Reflection;
using System.IO;

namespace Leafing.Core
{
	public static class ResourceHelper
	{
		private static readonly Assembly Assembly = Assembly.GetEntryAssembly();

		#region GetStream

        public static Stream GetStream(Assembly asm, string filePath, bool addPrefix)
		{
            string fn = (addPrefix) ? string.Format("{0}.{1}", asm.GetName().Name, filePath) : filePath;
			return asm.GetManifestResourceStream(fn);
		}

        public static Stream GetStream(Assembly asm, string filePath)
        {
            return GetStream(asm, filePath, true);
        }

        public static Stream GetStream(Type t, string filePath)
		{
			var asm = Assembly.GetAssembly(t);
			return GetStream(asm, filePath);
		}

		public static Stream GetStream(string filePath)
		{
			string fn = string.Format("{0}.{1}",  Assembly.GetName().Name, filePath);
			return Assembly.GetManifestResourceStream(fn);
		}

		#endregion

		#region GetStreamReader

        public static StreamReader GetStreamReader(Assembly asm, string filePath, bool addPrefix)
        {
            return new StreamReader(GetStream(asm, filePath, addPrefix));
        }

        public static StreamReader GetStreamReader(Assembly asm, string filePath)
		{
			return new StreamReader(GetStream(asm, filePath));
		}

		public static StreamReader GetStreamReader(Type t, string filePath)
		{
			var asm = Assembly.GetAssembly(t);
			return GetStreamReader(asm, filePath);
		}

		public static StreamReader GetStreamReader(string filePath)
		{
			return GetStreamReader(Assembly, filePath);
		}

		#endregion

		#region ReadToEnd

		public static string ReadToEnd(Assembly asm, string filePath, bool addPrefix)
		{
            using (var sr = GetStreamReader(asm, filePath, addPrefix))
			{
				return sr.ReadToEnd();
			}
		}

        public static string ReadToEnd(Assembly asm, string filePath)
        {
            return ReadToEnd(asm, filePath, true);
        }

        public static string ReadToEnd(Type t, string filePath)
		{
			var asm = Assembly.GetAssembly(t);
			return ReadToEnd(asm, filePath);
		}

		public static string ReadToEnd(string filePath)
		{
			return ReadToEnd(Assembly, filePath);
		}

        public static byte[] ReadAll(Type t, string filePath)
        {
            return ReadAll(GetStream(t, filePath));
        }

        public static byte[] ReadAll(Assembly a, string filePath)
        {
            return ReadAll(GetStream(a, filePath));
        }

        public static byte[] ReadAll(string fullPath)
        {
            return ReadAll(Assembly.GetManifestResourceStream(fullPath));
        }

        public static byte[] ReadAll(Stream s)
        {
            using (s)
            {
                var l = (int)s.Length;
                var ret = new byte[l];
                s.Read(ret, 0, l);
                return ret;
            }
        }

        #endregion
	}
}
