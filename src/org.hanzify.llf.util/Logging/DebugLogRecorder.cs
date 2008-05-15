using System;
using System.Diagnostics;

namespace Lephone.Util.Logging
{
    public class DebugLogRecorder : ILogRecorder
    {
        public void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            DebugPrint("{0},{1},{2},{3},{4}", Type, Source, Name, Message, eException);
        }

        private static void DebugPrint(string msg, params object[] os)
        {
            ClassHelper.CallFunction(typeof(Debug), "Print", msg, os);
        }
    }
}
