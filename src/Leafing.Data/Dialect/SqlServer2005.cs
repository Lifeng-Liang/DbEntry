using System.Collections.Generic;
using System.Data;
using System.Text;
using Leafing.Data.Builder;
using Leafing.Data.Model;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Dialect
{
    public class SqlServer2005 : SqlServer2000
    {
        public SqlServer2005()
        {
            TypeNames[DataType.String] = "VARCHAR";
            TypeNames[DataType.Binary] = "VARBINARY";
        }

        protected override string GetStringNameWithLength(string baseType, bool isUnicode, int length)
        {
            if(length == 0)
            {
                return isUnicode ? "NVARCHAR(MAX)" : "VARCHAR(MAX)";
            }
            return base.GetStringNameWithLength(baseType, isUnicode, length);
        }

        protected override string GetBinaryNameWithLength(string baseType, int length)
        {
            if(length == 0)
            {
                return "VARBINARY(MAX)";
            }
            return base.GetBinaryNameWithLength(baseType, length);
        }

        public override bool SupportsRangeStartIndex
        {
            get { return true; }
        }

        public override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb, List<string> queryRequiredFields)
        {
            if (ssb.Order == null || ssb.Keys.Count == 0)
            {
                throw PagedMustHaveOrder;
            }

            const string posName = "__rownumber__";
            var dpc = new DataParameterCollection();
            string sqlString = string.Format(
				"SELECT {7} FROM (SELECT {0}, ROW_NUMBER() OVER ({3}) AS {6} FROM {1} {2}) AS T WHERE T.{6} >= {4} AND T.{6} <= {5} ORDER BY T.{6}",
                ssb.GetColumns(this),
                ssb.From.ToSqlText(dpc, this),
                ssb.Where.ToSqlText(dpc, this, queryRequiredFields),
                ssb.Order.ToSqlText(dpc, this),
                ssb.Range.StartIndex,
                ssb.Range.EndIndex,
                posName,
                ssb.GetColumns(this, false, true)
                );
            return new TimeConsumingSqlStatement(CommandType.Text, sqlString, dpc);
        }

        public override SqlStatement GetAddDescriptionSql(ObjectInfo info)
        {
            const string template = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{2}' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'{0}', @level2type=N'COLUMN', @level2name=N'{1}';\n";
            var sb = new StringBuilder();
            string table = info.From.MainTableName;
            foreach (var field in info.SimpleMembers)
            {
                if (!field.Description.IsNullOrEmpty())
                {
                    sb.Append(string.Format(template, table, field.Name, field.Description.Replace("'", "''")));
                }
            }
            if (sb.Length > 0)
            {
                return new SqlStatement(sb.ToString());
            }
            return null;
        }
    }
}
