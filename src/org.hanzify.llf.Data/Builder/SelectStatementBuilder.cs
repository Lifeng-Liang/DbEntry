
#region usings

using System;
using System.Text;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.Builder.Clause;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.Builder
{
	public class SelectStatementBuilder : ISqlStatementBuilder, ISqlValues, ISqlWhere
	{
        private OrderBy _Order;
        private Range _Limit;
        private FromClause _From;
        private WhereClause _WhereOptions = new WhereClause();

        private KeyValueCollection kvc = new KeyValueCollection();

        internal string CountCol = null;

        internal bool IsGroupBy = false;

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
            CountCol = ColumnName;
        }

        public void SetAsGroupBy(string ColumnName)
        {
            SetCountColumn(ColumnName);
            IsGroupBy = true;
        }
        
        public SqlStatement ToSqlStatement(DbDialect dd)
		{
            if (Values.Count == 0 && _Limit != null)
            {
                throw new DbEntryException("When Values is empty, It means Get Count, Limit must be null.");
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
			StringBuilder Columns = new StringBuilder();
			foreach ( KeyValue kv in kvc )
			{
				Columns.Append(dd.QuoteForColumnName(kv.Key));
                Columns.Append(",");
			}
            if (CountCol != null)
            {
                if (CountCol == "*")
                {
                    Columns.Append("Count(*) As ").Append(DbEntry.CountColumn).Append(",");
                }
                else
                {
                    Columns.Append("Count(")
                        .Append(dd.QuoteForColumnName(CountCol))
                        .Append(") As ").Append(DbEntry.CountColumn).Append(",");
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

        public KeyValueCollection Values
		{
			get { return kvc; }
		}

		public WhereClause Where
		{
			get { return _WhereOptions; }
		}
	}
}
