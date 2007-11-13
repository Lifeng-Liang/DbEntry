
using System;

namespace Lephone.Util.Text
{
    public class InflectionNameMapper : UnderlineNameMapper
    {
        public override string MapName(string Name)
        {
            return Inflector.Tableize(Name);
        }
    }
}
