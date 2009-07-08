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
        private readonly OrderBy _Order;
        private readonly Range _Limit;
        private readonly FromClause _From;
        private readonly WhereClause _WhereOptions = new WhereClause();

        private readonly List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string, string>>();

        internal List<string> FunctionArgs = new List<string>();
        internal string FunctionName;

        internal bool IsGroupBy;
        internal bool IsDistinct;

		public SelectStatementBuilder(string TableName) : this(TableName, null, null)
		{
		}

        public SelectStatementBuilder(string TableName, OrderBy Order, Range Limit)
            : this(new FromClause(TableName), Order, Limit)
		{
		}

        public SelectStatementBuilder(FromClause From, OrderBy Order, Range Limit)
        {
            _From = From;
            _Limit = Limit;
            _Order = Order;
        }

        public void SetCountColumn(string ColumnName)
        {
            SetFunctionColumn("COUNT", ColumnName);
        }

        public void SetMaxColumn(string ColumnName)
        {
            SetFunctionColumn("MAX", ColumnName);
        }

        public void SetMinColumn(string ColumnName)
        {
            SetFunctionColumn("MIN", ColumnName);
        }

        public void SetSumColumn(string ColumnName)
        {
            SetFunctionColumn("SUM", ColumnName);
        }

        private void SetFunctionColumn(string FunctionName, string ColumnName)
        {
            this.FunctionName = FunctionName;
            FunctionArgs.Add(ColumnName);
        }

        public void SetAsGroupBy(string ColumnName)
        {
            SetCountColumn(ColumnName);
            IsGroupBy = true;
        }
        
        public SqlStatement ToSqlStatement(DbDialect dd)
		{
            if (Keys.Count == 0 && _Limit != null)
            {
                throw new DataException("When Values is empty, It means Get Count, Limit must be null.");
            }
            SqlStatement sql = dd.GetSelectSqlStatement(this);
            if (_Limit != null)
            {
                sql.StartIndex = _Limit.StartIndex;
                sql.EndIndex = _Limit.EndIndex;
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
			foreach ( var k in keys )
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

        public Range Range
        {
            get { return _Limit; }
        }

        public OrderBy Order
        {
            get { return _Order; }
        }

        public FromClause From
        {
            get { return _From; }
        }

        public List<KeyValuePair<string, string>> Keys
		{
			get { return keys; }
		}

		public WhereClause Where
		{
			get { return _WhereOptions; }
		}
	}
}
