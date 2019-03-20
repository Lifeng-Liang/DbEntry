using System.Collections.Generic;
using Leafing.Core.Ioc;

namespace Leafing.Core.Logging {
    static class LogRecorderProvider {
        private static readonly Dictionary<string, ILogRecorder> Jar = new Dictionary<string, ILogRecorder>();

        public static ILogRecorder GetLogRecorder(string name) {
            lock (Jar) {
                if (Jar.ContainsKey(name)) {
                    return Jar[name];
                }
                var ilc = name.StartsWith("@")
                    ? SimpleContainer.Get<ILogRecorder>(name.Substring(1))
                    : (ILogRecorder)ClassHelper.CreateInstance(name);
                if (ilc == null) {
                    throw new SettingException();
                }
                Jar[name] = ilc;
                return ilc;
            }
        }
    }
}
