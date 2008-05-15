using System;
using Lephone.Util.Logging;

namespace Lephone.Data.Logging
{
    public class DatabaseLogRecorder : ILogRecorder
    {
        public void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            LephoneLog li = new LephoneLog(Type, Source, Name, Message, eException);
            try
            {
                DbEntry.Context.Insert(li);
            }
            catch (Exception ex)
            {
                string msg = (eException == null) ? Message : Message + "\n" + eException;
                ((ILogDirect)Logger.System).Log(Type, Source, Name, msg, ex);
            }
        }
    }
}
