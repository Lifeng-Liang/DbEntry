using System.Collections.Generic;
using System.Text;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;
using Leafing.Data.SqlEntry;
using Leafing.Data.Common;

namespace Leafing.Data.Dialect
{
    public class Firebird : SequencedDialect
    {
        public Firebird()
        {
            TypeNames[DataType.Boolean] = "SMALLINT";
            TypeNames[DataType.DateTime] = "TIMESTAMP";
            TypeNames[DataType.Time] = "TIME";
            TypeNames[DataType.Binary] = "BLOB";
            TypeNames[DataType.Double] = "DOUBLE PRECISION";
            TypeNames[DataType.Guid] = "CHAR(38)";
        }

        protected override string GetStringNameWithLength(string baseType, bool isUnicode, int length)
        {
            if (length == 0)
            {
                return "BLOB SUB_TYPE TEXT";
            }
            if (isUnicode)
            {
                return baseType + " (" + length + ") CHARACTER SET UNICODE_FSS";
            }
            return baseType + " (" + length + ")";
        }

        protected override string GetBinaryNameWithLength(string baseType, int length)
        {
            return "BLOB SUB_TYPE BINARY";
        }

        public override string DbNowString
        {
            get { return "CURRENT_TIMESTAMP"; }
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new[] { null, null, null, "TABLE" }, null, null, null);
        }

        public override string GetSelectSequenceSql(string tableName)
        {
            return string.Format("SELECT GEN_ID(GEN_{0}_ID, 1) FROM RDB$DATABASE", tableName.ToUpper());
        }

        public override bool NeedCommitCreateFirst
        {
            get { return true; }
        }

        public override bool SupportDirctionOfEachColumnInIndex
        {
            get { return false; }
        }

        public override string IdentityColumnString
        {
            get { return "NOT NULL"; }
        }

        public override string GetCreateSequenceString(string tableName)
        {
            return string.Format("CREATE GENERATOR GEN_{0}_ID;\n", tableName.ToUpper());
        }

        protected override string QuoteSingle(string name)
        {
            return base.QuoteSingle(name.ToUpper());
        }

        public override string NullString
        {
            get { return ""; }
        }

        public override bool ExecuteEachLine
        {
            get { return true; }
        }

        public override void ExecuteDropSequence(DataProvider dp, string tableName)
        {
            string sql = string.Format("DROP GENERATOR GEN_{0}_ID;\n", tableName.ToUpper());
            dp.ExecuteNonQuery(sql);
        }

        public override SqlStatement GetPagedSelectSqlStatement(Builder.SelectStatementBuilder ssb, List<string> queryRequiredFields)
        {
            SqlStatement sql = ssb.GetNormalSelectSqlStatement(this, queryRequiredFields);
            sql.SqlCommandText = string.Format("{0} ROWS {1} TO {2}",
                sql.SqlCommandText, ssb.Range.StartIndex, ssb.Range.EndIndex);
            return sql;
        }

        public override string GenIndexName(string n)
        {
            return GenIndexName(n, 31);
        }

        public override void AddColumn(ModelContext ctx, string columnName, object o)
        {
            base.AddColumn(ctx, columnName, null);
            if (o != null)
            {
                var stm = new UpdateStatementBuilder(ctx.Info.From);
                stm.Values.Add(new KeyOpValue(columnName, o, KvOpertation.None));
                var sql = stm.ToSqlStatement(ctx);
                ctx.Provider.ExecuteNonQuery(sql);
            }
        }

        public override void DropColumns(ModelContext ctx, params string[] columns)
        {
            foreach(var column in columns)
            {
                var sb = new StringBuilder("ALTER TABLE ");
                sb.Append(QuoteForTableName(ctx.Info.From.MainTableName));
                sb.Append(" DROP ");
                sb.Append(QuoteForColumnName(column));
                sb.Append(";");
                var sql = new SqlStatement(sb.ToString());
                ctx.Provider.ExecuteNonQuery(sql);
            }
        }
    }
}
