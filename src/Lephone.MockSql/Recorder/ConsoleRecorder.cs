using System;

namespace Lephone.MockSql.Recorder
{
    public class ConsoleRecorder : IRecorder
    {
        public void Write(string msg, params object[] os)
        {
            Console.WriteLine(msg, os);
        }
    }
}
