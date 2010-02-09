using System.Collections.Generic;

namespace Lephone.Web.Mvc
{
    public class FlashHandler : SessionHandler
    {
        public override object this[string name]
        {
            get
            {
                Dictionary<string, object> bag = GetCurrentBag();
                if (bag.ContainsKey(name))
                {
                    object o = bag[name];
                    bag.Remove(name);
                    return o;
                }
                return string.Empty;
            }
            set
            {
                base[name] = value;
            }
        }

        public string Tip
        {
            get
            {
                return (string)this["Tip"];
            }
            set
            {
                this["Tip"] = value;
            }
        }

        public string Notice
        {
            get
            {
                return (string)this["Notice"];
            }
            set
            {
                this["Notice"] = value;
            }
        }

        public string Warning
        {
            get
            {
                return (string)this["Warning"];
            }
            set
            {
                this["Warning"] = value;
            }
        }
    }
}


