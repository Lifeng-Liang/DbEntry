using System;
using Leafing.Core.Text;
using Leafing.Data.SqlEntry;
using Leafing.Data.Definition;

namespace Leafing.Data.Builder.Clause
{
    public class JoinClause : IClause
    {
        public Type Type1;
        public string Table1;
        public string Key1;
        public Type Type2;
        public string Table2;
        public string Key2;
        public CompareOpration Comp;
        public JoinMode Mode;

        public JoinClause(Type type1, string key1, Type type2, string key2, CompareOpration comp, JoinMode mode)
        {
            this.Type1 = type1;
            this.Type2 = type2;
            this.Table1 = ModelContext.GetInstance(type1).Info.From.MainTableName;
            this.Table2 = ModelContext.GetInstance(type2).Info.From.MainTableName;
            this.Key1 = key1;
            this.Key2 = key2;
            this.Comp = comp;
            this.Mode = mode;
        }

        public JoinClause(string table1, string key1, string table2, string key2, CompareOpration comp, JoinMode mode)
        {
            this.Table1 = table1;
            this.Table2 = table2;
            this.Key1 = key1;
            this.Key2 = key2;
            this.Comp = comp;
            this.Mode = mode;
        }

        public string ToSqlText(DataParameterCollection dpc, Dialect.DbDialect dd)
        {
            return string.Format("{0}.{1} {4} {2}.{3}",
                dd.QuoteForTableName(Table1),
                dd.QuoteForColumnName(Key1),
                dd.QuoteForTableName(Table2),
                dd.QuoteForColumnName(Key2),
                StringHelper.EnumToString(Comp));
        }
    }
}
