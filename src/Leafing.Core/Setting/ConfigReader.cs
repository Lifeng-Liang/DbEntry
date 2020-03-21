using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Leafing.Core.Text;

namespace Leafing.Core.Setting {
    public static class ConfigReader {
        public static readonly Configuration Config;

        static ConfigReader() {
            var content = ReadFile("leafing.config.json") ?? ReadResource("leafing.config.json");
            Config = new Configuration();
            var newConfig = ParseConfigration(content);
            Config.Merge(newConfig);
        }

        private static string ReadFile(string fileName) {
            string result = null;
            if(File.Exists(fileName)) {
                Util.CatchAll(() => {
                    result = StringHelper.ReadToEnd(fileName);
                });
            }
            return result;
        }

        private static string ReadResource(string filePostfix) {
            string result = null;
            Util.CatchAll(() => {
                result = ResourceHelper.GetContent(fn => fn.EndsWith(filePostfix));
            });
            return result;
        }

        private static Configuration ParseConfigration(string s) {
            using (var ms = new MemoryStream()) {
                var bs = Encoding.UTF8.GetBytes(s);
                ms.Write(bs, 0, bs.Length);
                ms.Flush();
                ms.Position = 0;
                var ser = new DataContractJsonSerializer(typeof(Configuration), new DataContractJsonSerializerSettings() {
                    UseSimpleDictionaryFormat = true,
                    SerializeReadOnlyTypes = true,
                });
                var config = (Configuration)ser.ReadObject(ms);
                return config;
            }
        }
    }
}
