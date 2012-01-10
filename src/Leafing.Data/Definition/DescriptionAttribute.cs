using System;

namespace Leafing.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Text;

        public DescriptionAttribute(string text)
        {
            if(text.IsNullOrEmpty())
            {
                throw new ArgumentNullException("text");
            }
            this.Text = text;
        }
    }
}
