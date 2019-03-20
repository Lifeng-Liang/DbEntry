using System.Collections.Generic;
using Leafing.Core.Setting;

namespace Leafing.MockSql.Recorder {
    public class StaticRecorder : IRecorder {
        public static int ConnectionOpendTimes;

        public static readonly List<RowInfo> CurRow = new List<RowInfo>();

        private static string _lastMessage = "";

        public static string LastMessage {
            get { return _lastMessage; }
        }

        public static readonly List<string> Messages = new List<string>();

        public static void ClearMessages() {
            _lastMessage = "";
            Messages.Clear();
        }

        public void Write(string msg, params object[] os) {
            if (ConfigReader.Config.Database.UseForeignKey && msg.StartsWith("PRAGMA foreign_keys = ON;")) {
                return;
            }
            _lastMessage = string.Format(msg, os);
            Messages.Add(_lastMessage);
        }
    }
}