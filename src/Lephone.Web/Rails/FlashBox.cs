using System.Collections.Generic;

namespace Lephone.Web.Rails
{
    public class FlashBox : SessionBox
    {
        public override object this[string Name]
        {
            get
            {
                Dictionary<string, object> bag = GetCurrentBag();
                if (bag.ContainsKey(Name))
                {
                    object o = bag[Name];
                    bag.Remove(Name);
                    return o;
                }
                return string.Empty;
            }
            set
            {
                base[Name] = value;
            }
        }
    }
}
