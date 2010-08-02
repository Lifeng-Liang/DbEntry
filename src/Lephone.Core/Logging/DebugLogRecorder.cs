using System;
using System.Diagnostics;

namespace Lephone.Core.Logging
{
    public class DebugLogRecorder : ILogRecorder
    {
        public void ProcessLog(SysLogType type, string source, string name, string message, Exception exception)
        {
            DebugPrint("{0},{1},{2},{3},{4}", type, source, name, message, exception);
        }

        private static void DebugPrint(string msg, params object[] os)
        {
            ClassHelper.CallFunction(typeof(Debug), "Print", msg, os);
        }
    }
}
