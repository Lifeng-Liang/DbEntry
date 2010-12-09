using System.Collections.Generic;
using System.Text;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;
using Lephone.Data.SqlEntry;
using Lephone.Data.Common;

namespace Lephone.Data.Builder
{
	public class SelectStatementBuilder : ISqlStatementBuilder, ISqlKeys, ISqlWhere
	{
        private readonly OrderBy _order;
        private readonly Range _limit;
        private readonly FromClause _from;
        private readonly WhereClause _whereOptions = new WhereClause();

        private readonly List<KeyValuePair<string, string>> _keys = new List<KeyValuePair<string, string>>();

        internal List<string> GroupbyArgs = new List<string>();
        internal List<string> FunctionArgs = new List<string>();
        internal string FunctionName;

        internal bool IsGroupBy;
        internal bool IsDistinct;
	    internal bool NoLazy;

		public SelectStatementBuilder(string tableName) : this(tableName, null, null)
		{
		}

        public SelectStatementBuilder(string tableName, OrderBy order, Range limit)
            : this(new FromClause(tableName), order, limit)
		{
		}

        public SelectStatementBuilder(FromClause from, OrderBy order, Range limit)
        {
            _from = from;
            _limit = limit;
            _order = order;
        }

        public void SetCountColumn(string columnName)
        {
            SetFunctionColumn("COUNT", columnName);
        }

        public void SetMaxColumn(string columnName)
        {
            SetFunctionColumn("MAX", columnName);
        }

        public void SetMinColumn(string columnName)
        {
            SetFunctionColumn("MIN", columnName);
        }

        public void SetSumColumn(string columnName)
        {
            SetFunctionColumn("SUM", columnName);
        }

        private void SetFunctionColumn(string functionName, string columnName)
        {
            FunctionName = functionName;
            FunctionArgs.Add(columnName);
        }

        public void SetAsGroupBy(string columnName)
        {
            GroupbyArgs.Add(columnName);
            SetCountColumn(columnName);
            IsGroupBy = true;
        }

        public void SetAsGroupBySum(string groupbyColumnName, string sumColumnName)
        {
            GroupbyArgs.Add(groupbyColumnName);
            SetSumColumn(sumColumnName);
            IsGroupBy = true;
        }

        public SqlStatement ToSqlStatement(DbDialect dd)
		{
            if (Keys.Count == 0 && _limit != null)
            {
                throw new DataException("When Values is empty, It means Get Count, Limit must be null.");
            }
            SqlStatement sql = dd.GetSelectSqlStatement(this);
            if (_limit != null)
            {
                sql.StartIndex = _limit.StartIndex;
                sql.EndIndex = _limit.EndIndex;
            }
            return sql;
		}

        internal string GetColumns(DbDialect dd)
        {
            return GetColumns(dd, true, true);
        }

        internal string GetColumns(DbDialect dd, bool includeOrigin, bool includeAlias)
		{
			var columns = new StringBuilder();
            if(IsDistinct)
            {
                columns.Append("DISTINCT ");
            }
			foreach ( var k in _keys )
			{
                AddColumn(dd, columns, includeOrigin, includeAlias, k);
			}
            //if(IsGroupBy)
            //{
            //    if (GroupbyArgs.Count != 0)
            //    {
            //        if (GroupbyArgs[0] == "*" || GroupbyArgs.Count > 1)
            //        {
            //        }
            //        else
            //        {
            //            string fa = GroupbyArgs[0];
            //            string fn = fa.StartsWith("DISTINCT ") ? fa : dd.QuoteForColumnName(fa);
            //            string gfn = FunctionName == "COUNT" ? DbEntry.CountColumn : fn;
            //            columns.Append("(")
            //                .Append(fn)
            //                .Append(") AS ").Append(gfn).Append(",");
            //        }
            //    }
            //}
            if (FunctionArgs.Count != 0)
            {
                columns.Append(FunctionName);
                if (FunctionArgs[0] == "*" || FunctionArgs.Count > 1)
                {
                    columns.Append("(*) AS ").Append(DbEntry.CountColumn).Append(",");
                }
                else
                {
                    string fa = FunctionArgs[0];
                    string fn = fa.StartsWith("DISTINCT ") ? fa : dd.QuoteForColumnName(fa);
                    string gfn = FunctionName == "COUNT" ? DbEntry.CountColumn : fn;
                    columns.Append("(")
                        .Append(fn)
                        .Append(") AS ").Append(gfn).Append(",");
                }
            }
            if (columns.Length > 0)
            {
                columns.Length--;
            }
            return columns.ToString();
		}

	    private static void AddColumn(DbDialect dd, StringBuilder columns, bool includeOrigin, bool includeAlias, KeyValuePair<string, string> k)
	    {
	        if (includeOrigin)
	        {
	            columns.Append(dd.QuoteForColumnName(k.Key));
	            if (includeAlias && k.Value != null) { columns.Append(" AS "); }
	        }
	        if (includeAlias)
	        {
	            if (k.Value != null)
	            {
	                columns.Append(dd.QuoteForColumnName(k.Value));
	            }
	            else if (!includeOrigin)
	            {
	                columns.Append(dd.QuoteForColumnName(k.Key));
	            }
	        }
	        columns.Append(",");
	    }

	    public Range Range
        {
            get { return _limit; }
        }

        public OrderBy Order
        {
            get { return _order; }
        }

        public FromClause From
        {
            get { return _from; }
        }

        public List<KeyValuePair<string, string>> Keys
		{
			get { return _keys; }
		}

		public WhereClause Where
		{
			get { return _whereOptions; }
		}
	}
}
