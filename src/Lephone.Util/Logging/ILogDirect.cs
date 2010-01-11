using System;

namespace Lephone.Util.Logging
{
    public interface ILogDirect
    {
        void Log(LogType type, string name, string message);
        void Log(LogType type, string source, string name, string message);
        void Log(LogType type, string name, string message, Exception eException);
        void Log(LogType type, string source, string name, string message, Exception eException);
    }
}
