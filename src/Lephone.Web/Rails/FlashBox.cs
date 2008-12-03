namespace Lephone.Web.Rails
{
    public class FlashBox : SessionBox
    {
        public override object this[string Name]
        {
            get
            {
                object o = base[Name];
                if(o != null)
                {
                    BagSet.Remove(Name);
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
