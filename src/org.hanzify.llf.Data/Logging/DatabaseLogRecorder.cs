
#region usings

using System;
using org.hanzify.llf.util.Logging;

#endregion

namespace org.hanzify.llf.Data.Logging
{
    public class DatabaseLogRecorder : ILogRecorder
    {
        public void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            LogItem li = new LogItem(Type, Source, Name, Message, eException);
            try
            {
                DbEntry.Context.UsingConnection(delegate()
                {
                    DbEntry.Save(li);
                });
            }
            catch (Exception ex)
            {
                string msg = (eException == null) ? Message : Message + "\n" + eException.ToString();
                ((ILogDirect)Logger.System).Log(Type, Source, Name, msg, ex);
            }
        }
    }
}
