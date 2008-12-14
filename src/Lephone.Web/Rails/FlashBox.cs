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

        public string Tip
        {
            get
            {
                return (string)this["tip"];
            }
            set
            {
                this["tip"] = value;
            }
        }

        public string Notice
        {
            get
            {
                return (string)this["notice"];
            }
            set
            {
                this["notice"] = value;
            }
        }

        public string Warning
        {
            get
            {
                return (string)this["warning"];
            }
            set
            {
                this["warning"] = value;
            }
        }
    }
}
