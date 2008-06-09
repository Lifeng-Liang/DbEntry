using System.Collections.Generic;
using System.Collections.Specialized;
using Lephone.Util.Setting;

namespace Lephone.UnitTest.util
{
    public class MockConfigReader : ConfigReader
    {
        private Dictionary<string, NameValueCollection> dict;

        public MockConfigReader(params KeyValuePair<string, NameValueCollection>[] nvcs)
        {
            dict = new Dictionary<string, NameValueCollection>();
            foreach (KeyValuePair<string, NameValueCollection> n in nvcs)
            {
                dict.Add(n.Key, n.Value);
            }
            //SetInstance(this);
        }

        public override NameValueCollection GetSection(string SectionName)
        {
            if (dict.ContainsKey(SectionName))
            {
                return dict[SectionName];
            }
            else
            {
                return new NameValueCollection();
            }
        }
    }
}
