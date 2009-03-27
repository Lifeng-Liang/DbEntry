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

        private readonly List<string> keys = new List<string>();

        internal string FunctionCol;
        internal string FunctionName;

        internal bool IsGroupBy;

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
            FunctionCol = ColumnName;
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
            SqlStatement Sql = dd.GetSelectSqlStatement(this);
            if (_Limit != null)
            {
                Sql.StartIndex = _Limit.StartIndex;
                Sql.EndIndex = _Limit.EndIndex;
            }
            return Sql;
		}

        internal string GetColumns(DbDialect dd)
		{
			var Columns = new StringBuilder();
			foreach ( string k in keys )
			{
				Columns.Append(dd.QuoteForColumnName(k));
                Columns.Append(",");
			}
            if (FunctionCol != null)
            {
                Columns.Append(FunctionName);
                if (FunctionCol == "*")
                {
                    Columns.Append("(*) AS ").Append(DbEntry.CountColumn).Append(",");
                }
                else
                {
                    string fn = dd.QuoteForColumnName(FunctionCol);
                    string gfn = FunctionName == "COUNT" ? DbEntry.CountColumn : fn;
                    Columns.Append("(")
                        .Append(fn)
                        .Append(") AS ").Append(gfn).Append(",");
                }
            }
            if (Columns.Length > 0)
            {
                Columns.Length--;
            }
            return Columns.ToString();
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

        public List<string> Keys
		{
			get { return keys; }
		}

		public WhereClause Where
		{
			get { return _WhereOptions; }
		}
	}
}
