using Lephone.Data.SqlEntry;
using Lephone.Core.Logging;
using Lephone.Data.Common;

namespace Lephone.Data.Dialect
{
    public class Firebird : SequencedDialect
    {
        public Firebird()
        {
            TypeNames[DataType.Boolean] = "SMALLINT";
            TypeNames[DataType.DateTime] = "TIMESTAMP";
            TypeNames[DataType.Time] = "TIME";
            TypeNames[DataType.String] = "BLOB SUB_TYPE 1";
            TypeNames[DataType.Binary] = "BLOB SUB_TYPE 0";
            TypeNames[typeof(byte[])] = "BLOB";
            TypeNames[DataType.Double] = "DOUBLE PRECISION";
            TypeNames[DataType.Guid] = "CHAR(38)";
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

        public override string GetUnicodeTypeString(string asciiTypeString)
        {
            return asciiTypeString + " CHARACTER SET UNICODE_FSS";
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
            Logger.SQL.Trace(sql);
            dp.ExecuteNonQuery(sql);
        }

        protected override SqlStatement GetPagedSelectSqlStatement(Builder.SelectStatementBuilder ssb)
        {
            SqlStatement sql = base.GetNormalSelectSqlStatement(ssb);
            sql.SqlCommandText = string.Format("{0} ROWS {1} TO {2}",
                sql.SqlCommandText, ssb.Range.StartIndex, ssb.Range.EndIndex);
            return sql;
        }

        public override string GenIndexName(string n)
        {
            return GenIndexName(n, 31);
        }

        protected override string GetLengthStringForBlob(int length)
        {
            if(length < 80)
            {
                return base.GetLengthStringForBlob(length);
            }
            return "";
        }
    }
}
