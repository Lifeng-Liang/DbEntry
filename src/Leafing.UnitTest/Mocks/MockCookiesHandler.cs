using System.Collections.Generic;
using Leafing.Core.Ioc;
using Leafing.Web.Mvc.Core;

namespace Leafing.UnitTest.Mocks
{
    [Implementation(2)]
    public class MockCookiesHandler : CookiesHandler
    {
        private readonly Dictionary<string, string> _bag = new Dictionary<string, string>();

        public override string this[string name]
        {
            get
            {
                if(_bag.ContainsKey(name))
                {
                    return _bag[name];
                }
                return null;
            }
            set
            {
                _bag[name] = value;
            }
        }

        public void Clear()
        {
            _bag.Clear();
        }
    }
}
