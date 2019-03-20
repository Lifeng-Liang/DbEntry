using System;
using System.Collections.Generic;
using Leafing.Core.Logging;

namespace Leafing.UnitTest {
    public class SqlRecorder : ILogRecorder {
        private static bool _running;
        public static List<string> List = new List<string>();

        public static string LastMessage {
            get {
                return List.Count > 0 ? List[List.Count - 1] : null;
            }
        }

        public static void Start() {
            List.Clear();
            _running = true;
        }

        public static void Stop() {
            _running = false;
            List.Clear();
        }

        public void ProcessLog(LogLevel type, string name, string message, Exception exception) {
            if (_running) {
                List.Add(message);
            }
        }
    }
}