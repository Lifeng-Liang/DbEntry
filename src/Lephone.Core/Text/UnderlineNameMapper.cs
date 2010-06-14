using System.Text;

namespace Lephone.Core.Text
{
    public class UnderlineNameMapper : NameMapper
    {
        public override string MapName(string name)
        {
            var sb = new StringBuilder();
            bool beforeIsSmall = false;
            foreach (char c in name)
            {
                if (beforeIsSmall && IsLarge(c))
                {
                    sb.Append("_");
                }
                beforeIsSmall = IsSmall(c);
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static bool IsLarge(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        private static bool IsSmall(char c)
        {
            return c >= 'a' && c <= 'z';
        }

        public override string Prefix
        {
            get { return "R_"; }
        }
    }
}
