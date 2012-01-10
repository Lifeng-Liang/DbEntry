using System.Collections.Generic;
using System.Collections.Specialized;
using Leafing.Core.Setting;

namespace Leafing.UnitTest.Mocks
{
    public class MockConfigReader : ConfigReader
    {
        private readonly Dictionary<string, NameValueCollection> _dict;

        public MockConfigReader(params KeyValuePair<string, NameValueCollection>[] nvcs)
        {
            _dict = new Dictionary<string, NameValueCollection>();
            foreach (KeyValuePair<string, NameValueCollection> n in nvcs)
            {
                _dict.Add(n.Key, n.Value);
            }
            //SetInstance(this);
        }

        public override NameValueCollection GetSection(string sectionName)
        {
            if (_dict.ContainsKey(sectionName))
            {
                return _dict[sectionName];
            }
            return new NameValueCollection();
        }
    }
}
