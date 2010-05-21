using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Text;

        public DescriptionAttribute(string text)
        {
            this.Text = text;
        }
    }
}
