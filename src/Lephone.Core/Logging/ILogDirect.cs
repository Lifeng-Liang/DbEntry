using System;

namespace Lephone.Core.Logging
{
    public interface ILogDirect
    {
        void Log(SysLogType type, string name, string message);
        void Log(SysLogType type, string source, string name, string message);
        void Log(SysLogType type, string name, string message, Exception exception);
        void Log(SysLogType type, string source, string name, string message, Exception exception);
    }
}
