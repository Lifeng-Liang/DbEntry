
using System;
using System.Data;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public class SqlServer2005 : SqlServer2000
    {
        public override bool SupportsRangeStartIndex
        {
            get { return true; }
        }

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            if (ssb.Order == null || ssb.Keys.Count == 0)
            {
                throw new DbEntryException("Paged select must have Order And Values not Empty.");
            }

            const string PosName = "__rownumber__";
            DataParamterCollection dpc = new DataParamterCollection();
            string SqlString = string.Format(
                "select {0} from (select {0}, ROW_NUMBER() OVER ({3}) as {6} From {1} {2}) as T Where T.{6} >= {4} and T.{6} <= {5}",
                ssb.GetColumns(this),
                ssb.From.ToSqlText(dpc, this),
                ssb.Where.ToSqlText(dpc, this),
                ssb.Order.ToSqlText(dpc, this),
                ssb.Range.StartIndex,
                ssb.Range.EndIndex,
                PosName
                );
            return new TimeConsumingSqlStatement(CommandType.Text, SqlString, dpc);
        }
    }
}
