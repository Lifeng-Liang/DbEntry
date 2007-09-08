
#region usings

using System;
using System.Collections.Generic;

#endregion

namespace Lephone.MockSql.Recorder
{
    public class StaticRecorder : IRecorder
    {
        public static List<string> CurRowNames;
        public static List<Type> CurRowTypes;
        public static List<object> CurRow;

        private static string _LastMessage = "";

        public static string LastMessage
        {
            get { return _LastMessage; }
        }

        private static List<string> _Messages = new List<string>();

        public static List<string> Messages
        {
            get { return _Messages; }
        }

        public static void ClearMessages()
        {
            _LastMessage = "";
            _Messages.Clear();
        }

        public void Write(string Msg, params object[] os)
        {
            _LastMessage = string.Format(Msg, os);
            _Messages.Add(_LastMessage);
        }
    }
}
