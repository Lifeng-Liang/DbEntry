using Leafing.Core.Ioc;

namespace Leafing.Core.Text
{
    [Implementation("Inflection")]
    public class InflectionNameMapper : UnderlineNameMapper
    {
        public override string MapName(string name)
        {
            return Inflector.Tableize(name);
        }

        public override string UnmapName(string name)
        {
            return Inflector.Classify(name);
        }
    }
}
