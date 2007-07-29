
#region usings

using System;

#endregion

namespace org.hanzify.llf.MockSql.Recorder
{
    public interface IRecorder
    {
        void Write(string Msg, params object[] os);
    }
}
