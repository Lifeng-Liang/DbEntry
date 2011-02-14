using System.Collections.Generic;
using Lephone.Core.Setting;

namespace Lephone.MockSql.Recorder
{
    public static class DataSettings
    {
        // ReSharper disable RedundantDefaultFieldInitializer
        public static readonly bool TestForeignKey = false;

        static DataSettings()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(DataSettings));
        }
        // ReSharper restore RedundantDefaultFieldInitializer
    }

    public class StaticRecorder : IRecorder
    {
        public static int ConnectionOpendTimes;

        public static readonly List<RowInfo> CurRow = new List<RowInfo>();

        private static string _lastMessage = "";

        public static string LastMessage
        {
            get { return _lastMessage; }
        }

        public static readonly List<string> Messages = new List<string>();

        public static void ClearMessages()
        {
            _lastMessage = "";
            Messages.Clear();
        }

        public void Write(string msg, params object[] os)
        {
            if (DataSettings.TestForeignKey && msg.StartsWith("PRAGMA foreign_keys = ON;"))
            {
                return;
            }
            _lastMessage = string.Format(msg, os);
            Messages.Add(_lastMessage);
        }
    }
}
