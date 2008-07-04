using System;
using System.Diagnostics;

namespace Lephone.Util.Logging
{
    public class DebugLogRecorder : ILogRecorder
    {
        public void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            DebugPrint("{0},{1},{2},{3},{4}", type, source, name, message, eException);
        }

        private static void DebugPrint(string msg, params object[] os)
        {
            ClassHelper.CallFunction(typeof(Debug), "Print", msg, os);
        }
    }
}
