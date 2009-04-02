using Lephone.Util.Text;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

namespace Lephone.Data.Builder.Clause
{
    public class JoinClause : IClause
    {
        public string Key1;
        public string Key2;
        public CompareOpration comp;
        public JoinMode mode;

        public JoinClause(string Key1, string Key2, CompareOpration comp, JoinMode mode)
        {
            this.Key1 = Key1;
            this.Key2 = Key2;
            this.comp = comp;
            this.mode = mode;
        }

        public string ToSqlText(DataParameterCollection dpc, Dialect.DbDialect dd)
        {
            return string.Format("{0} {2} {1}",
                dd.QuoteForColumnName(Key1),
                dd.QuoteForColumnName(Key2),
                StringHelper.EnumToString(comp));
        }
    }
}
