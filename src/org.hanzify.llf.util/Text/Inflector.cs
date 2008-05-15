using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lephone.Util.Text
{
    // Translate from ActiveSupport of Ruby On Rails
    public static class Inflector
    {
        private static readonly List<KeyValuePair<Regex, string>> Plurals;
        private static readonly List<KeyValuePair<Regex, string>> Singulars;
        private static readonly List<string> Uncountables;

        static Inflector()
        {
            Plurals = new List<KeyValuePair<Regex, string>>();
            Singulars = new List<KeyValuePair<Regex, string>>();
            Uncountables = new List<string>();
            Init();
        }

        private static void Plural(string rule, string replacement)
        {
            Plurals.Insert(0, new KeyValuePair<Regex, string>(
                new Regex(rule, RegexOptions.IgnoreCase), replacement));
        }

        private static void Singular(string rule, string replacement)
        {
            Singulars.Insert(0, new KeyValuePair<Regex, string>(
                new Regex(rule, RegexOptions.IgnoreCase), replacement));
        }

        private static void Irregular(string singular, string plural)
        {
            string s1 = singular[0].ToString();
            string s2 = singular.Substring(1);
            string p1 = plural[0].ToString();
            string p2 = plural.Substring(1);
            Plural("(" + s1 + ")" + s2 + "$", "$1" + p2);
            Singular("(" + p1 + ")" + p2 + "$", "$1" + s2);
        }

        private static void Uncountable(params string[] words)
        {
            Uncountables.AddRange(words);
        }

        public static string Pluralize(string word)
        {
            if (!Uncountables.Contains(word.ToLower()))
            {
                foreach (KeyValuePair<Regex, string> kv in Plurals)
                {
                    if (kv.Key.IsMatch(word))
                    {
                        return kv.Key.Replace(word, kv.Value);
                    }
                }
            }
            return word;
        }

        public static string Singularize(string word)
        {
            if (!Uncountables.Contains(word.ToLower()))
            {
                foreach (KeyValuePair<Regex, string> kv in Singulars)
                {
                    if (kv.Key.IsMatch(word))
                    {
                        return kv.Key.Replace(word, kv.Value);
                    }
                }
            }
            return word;
        }

        public static string Camelize(string lower_case_and_underscored_word)
        {
            return Camelize(lower_case_and_underscored_word, true);
        }

        public static string Camelize(string lower_case_and_underscored_word, bool first_letter_in_uppercase)
        {
            if (first_letter_in_uppercase)
            {
                string s = lower_case_and_underscored_word;
                s = new Regex(@"\/(.?)").Replace(s, delegate(Match match) { return "::" + match.Groups[1].Value.ToUpper(); });
                s = new Regex(@"(^|_)(.)").Replace(s, delegate(Match match) { return match.Groups[2].Value.ToUpper(); });
                return s;
            }
            return lower_case_and_underscored_word[0] + Camelize(lower_case_and_underscored_word).Substring(1);
        }

        public static string Titleize(string word)
        {
            string s = Humanize(Underscore(word));
            s = new Regex(@"\b([a-z])").Replace(s, delegate(Match match) { return StringHelper.Capitalize(match.Groups[1].Value); });
            return s;
        }

        public static string Underscore(string camel_cased_word)
        {
            string s = camel_cased_word;
            s = new Regex(@"::").Replace(s, @"/");
            s = new Regex(@"([A-Z]+)([A-Z][a-z])").Replace(s,@"$1_$2");
            s = new Regex(@"([a-z\d])([A-Z])").Replace(s, @"$1_$2");
            s = s.Replace('-', '_').ToLower();
            return s;
        }

        public static string Dasherize(string underscored_word)
        {
            return new Regex(@"_").Replace(underscored_word, "-");
        }

        public static string Humanize(string lower_case_and_underscored_word)
        {
            string s = lower_case_and_underscored_word;
            s = new Regex(@"_id$").Replace(s, "");
            s = new Regex(@"_").Replace(s, " ");
            return StringHelper.Capitalize(s);
        }

        public static string Demodulize(string class_name_in_module)
        {
            return new Regex(@"^.*::").Replace(class_name_in_module, "");
        }

        public static string Tableize(string class_name)
        {
            return Pluralize(Underscore(class_name));
        }

        public static string Classify(string table_name)
        {
            string s = new Regex(@".*\.").Replace(table_name, "");
            return Camelize(Singularize(s));
        }

        public static string ForeignKey(string class_name)
        {
            return ForeignKey(class_name, true);
        }

        public static string ForeignKey(string class_name, bool separate_class_name_and_id_with_underscore)
        {
            return Underscore(Demodulize(class_name)) + (separate_class_name_and_id_with_underscore ? "_id" : "id");
        }

        public static string Ordinalize(int number)
        {
            string n = number.ToString();
            if (CommonHelper.NewList(11, 12, 13).Contains(number % 100))
            {
                return n + "th";
            }
            switch (number % 10)
            {
                case 1: return n + "st";
                case 2: return n + "nd";
                case 3: return n + "rd";
                default: return n + "th";
            }
        }

        private static void Init()
        {
            Plural(@"$", @"s");
            Plural(@"s$", @"s");
            Plural(@"(ax|test)is$", @"$1es");
            Plural(@"(octop|vir)us$", @"$1i");
            Plural(@"(alias|status)$", @"$1es");
            Plural(@"(bu)s$", @"$1ses");
            Plural(@"(buffal|tomat)o$", @"$1oes");
            Plural(@"([ti])um$", @"$1a");
            Plural(@"sis$", @"ses");
            Plural(@"(?:([^f])fe|([lr])f)$", @"$1$2ves");
            Plural(@"(hive)$", @"$1s");
            Plural(@"([^aeiouy]|qu)y$", @"$1ies");
            Plural(@"(x|ch|ss|sh)$", @"$1es");
            Plural(@"(matr|vert|ind)ix|ex$", @"$1ices");
            Plural(@"([m|l])ouse$", @"$1ice");
            Plural(@"^(ox)$", @"$1en");
            Plural(@"(quiz)$", @"$1zes");

            Singular(@"s$", @"");
            Singular(@"(n)ews$", @"$1ews");
            Singular(@"([ti])a$", @"$1um");
            Singular(@"((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", @"$1$2sis");
            Singular(@"(^analy)ses$", @"$1sis");
            Singular(@"([^f])ves$", @"$1fe");
            Singular(@"(hive)s$", @"$1");
            Singular(@"(tive)s$", @"$1");
            Singular(@"([lr])ves$", @"$1f");
            Singular(@"([^aeiouy]|qu)ies$", @"$1y");
            Singular(@"(s)eries$", @"$1eries");
            Singular(@"(m)ovies$", @"$1ovie");
            Singular(@"(x|ch|ss|sh)es$", @"$1");
            Singular(@"([m|l])ice$", @"$1ouse");
            Singular(@"(bus)es$", @"$1");
            Singular(@"(o)es$", @"$1");
            Singular(@"(shoe)s$", @"$1");
            Singular(@"(cris|ax|test)es$", @"$1is");
            Singular(@"(octop|vir)i$", @"$1us");
            Singular(@"(alias|status)es$", @"$1");
            Singular(@"^(ox)en", @"$1");
            Singular(@"(vert|ind)ices$", @"$1ex");
            Singular(@"(matr)ices$", @"$1ix");
            Singular(@"(quiz)zes$", @"$1");

            Irregular("person", "people");
            Irregular("man", "men");
            Irregular("child", "children");
            Irregular("sex", "sexes");
            Irregular("move", "moves");

            Uncountable("equipment", "information", "rice", "money", "species", "series", "fish", "sheep");
        }
    }
}
