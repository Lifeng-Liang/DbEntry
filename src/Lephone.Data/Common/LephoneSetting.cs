using System.Collections.Generic;
using Lephone.Data.Definition;
using Lephone.Util.Setting;

namespace Lephone.Data.Common
{
    public class LephoneSetting : DbObjectModel<LephoneSetting>
    {
        [Length(1, 50), Index(UNIQUE = true)]
        public string Name { get; set; }

        public string Content { get; set; }
    }

    public class DbConfigHelper : ConfigHelperBase
    {
        public static readonly DbConfigHelper Instance = new DbConfigHelper();

        private readonly Dictionary<string, string> nvc = new Dictionary<string, string>();

        public DbConfigHelper()
        {
            LephoneSetting.FindAll().ForEach(p => nvc.Add(p.Name, p.Content));
        }

        protected override string GetString(string key)
        {
            return nvc[key];
        }
    }
}
