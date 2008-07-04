using System;

namespace Lephone.Util.Logging
{
    public interface ILogDirect
    {
        void Log(LogType Type, string Name, string Message);
        void Log(LogType Type, string Source, string Name, string Message);
        void Log(LogType Type, string Name, string Message, Exception eException);
        void Log(LogType Type, string Source, string Name, string Message, Exception eException);
    }
}
