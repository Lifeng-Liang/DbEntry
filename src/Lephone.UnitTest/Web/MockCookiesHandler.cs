using System.Collections.Generic;
using Lephone.Web.Rails;

namespace Lephone.UnitTest.Web
{
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
    }
}
