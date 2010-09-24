using System.Collections.Generic;

namespace Lephone.MockSql.Recorder
{
    public class StaticRecorder : IRecorder
    {
        public static readonly List<RowInfo> CurRow = new List<RowInfo>();

        private static string _lastMessage = "";

        public static string LastMessage
        {
            get { return _lastMessage; }
        }

        private static readonly List<string> _Messages = new List<string>();

        public static List<string> Messages
        {
            get { return _Messages; }
        }

        public static void ClearMessages()
        {
            _lastMessage = "";
            _Messages.Clear();
        }

        public void Write(string msg, params object[] os)
        {
            _lastMessage = string.Format(msg, os);
            _Messages.Add(_lastMessage);
        }
    }
}
