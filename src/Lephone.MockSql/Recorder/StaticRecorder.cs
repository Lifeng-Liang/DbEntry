using System.Collections.Generic;

namespace Lephone.MockSql.Recorder
{
    public class StaticRecorder : IRecorder
    {
        public static readonly List<RowInfo> CurRow = new List<RowInfo>();

        private static string _LastMessage = "";

        public static string LastMessage
        {
            get { return _LastMessage; }
        }

        private static readonly List<string> _Messages = new List<string>();

        public static List<string> Messages
        {
            get { return _Messages; }
        }

        public static void ClearMessages()
        {
            _LastMessage = "";
            _Messages.Clear();
        }

        public void Write(string msg, params object[] os)
        {
            _LastMessage = string.Format(msg, os);
            _Messages.Add(_LastMessage);
        }
    }
}
