using System.Text;

namespace Lephone.Util.Text
{
    public class UnderlineNameMapper : NameMapper
    {
        public override string MapName(string Name)
        {
            StringBuilder sb = new StringBuilder();
            bool BeforeIsSmall = false;
            foreach (char c in Name)
            {
                if (BeforeIsSmall && IsLarge(c))
                {
                    sb.Append("_");
                }
                BeforeIsSmall = IsSmall(c);
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
