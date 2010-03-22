using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Security;

namespace Lephone.Util
{
	public static class SystemHelper
	{
		private static string _exeFileName;
        private static string _baseDirectory;
	    private static string _tempDirectory;
	    private static string _currDirectory;

        public static string ExeFileName
        {
            get { return _exeFileName; }
        }

        public static string BaseDirectory
        {
            get { return _baseDirectory; }
        }

	    public static string TempDirectory
	    {
            get { return _tempDirectory; }
	    }

	    public static string CurrentDirectory
	    {
            get { return _currDirectory; }
	    }

		static SystemHelper()
		{
            CommonHelper.CatchAll(delegate
            {
                AppDomainSetup s = AppDomain.CurrentDomain.SetupInformation;
                _exeFileName = s.ApplicationName;
                _baseDirectory = s.ApplicationBase;
                _tempDirectory = Path.GetTempPath();
                _currDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (!_currDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    _currDirectory += Path.DirectorySeparatorChar;
                }
            });
		}

		public static string GetDateTimeString()
		{
			return GetDateTimeString(DateTime.Now);
		}

		public static string GetDateTimeString(DateTime dt)
		{
			return dt.ToString("yyyyMMdd_HHmmss");
		}

		#region Get StackTrack Function Name

		public static string CurrentFunctionName
		{
			get
			{
				return GetStackTrackFunctionName(0);
			}
		}

		public static string CallerFunctionName
		{
			get
			{
				return GetStackTrackFunctionName(1);
			}
		}

		private static string GetStackTrackFunctionName(int index)
		{
			index += 2;
			var st = new StackTrace(true);
			if ( st.FrameCount > index )
			{
				StackFrame sf = st.GetFrame(index);
				return GetMothodDesc(sf, false);
			}
		    return null;
		}

		private static string GetMothodDesc(StackFrame sf, bool needLineNo)
		{
			MethodBase mb = sf.GetMethod();
			if ( needLineNo )
			{
				return GetMothodLine(mb) + GetCurLine(sf);
			}
		    return GetMothodLine(mb);
		}

		private static string GetMothodLine(MethodBase mb)
		{
			var sb = new StringBuilder();
			Type t = mb.DeclaringType;
			if (t != null)
			{
				string s = t.Namespace;
				if (s != null)
				{
					sb.Append(s);
					sb.Append(".");
				}
				sb.Append(t.Name);
				sb.Append(".");
			}
			sb.Append(mb.Name);
			sb.Append("(");
			ParameterInfo[] pis = mb.GetParameters();
			for (int j = 0; j < pis.Length; j++)
			{
				string s = "<UnknownType>";
				if (pis[j].ParameterType != null)
				{
					s = pis[j].ParameterType.Name;
				}
				sb.Append(((j != 0) ? ", " : "") + s + " " + pis[j].Name);
			}
			sb.Append(")");
			return sb.ToString();
		}

		private static string GetCurLine(StackFrame sf)
		{
			if (sf.GetILOffset() != -1)
			{
				string s = null;
				try
				{
					s = sf.GetFileName();
				}
				catch (SecurityException)
				{
				}
				if (s != null)
				{
					return string.Format(" in {0}:line {1}", s, sf.GetFileLineNumber());
				}
			}
			return "";
		}

		#endregion
	}
}
