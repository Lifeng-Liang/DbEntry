namespace Lephone.Util.Text
{
    public class InflectionNameMapper : UnderlineNameMapper
    {
        public override string MapName(string Name)
        {
            return Inflector.Tableize(Name);
        }

        public override string UnmapName(string Name)
        {
            return Inflector.Classify(Name);
        }
    }
}
