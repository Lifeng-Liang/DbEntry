
#region usings

using System;
using System.Collections;
using System.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

#endregion

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public abstract class ConditionClause : WhereCondition
	{
		private string Condition;
		private ArrayList List = new ArrayList();

		public ConditionClause(string Condition)
		{
			this.Condition = Condition;
		}

		public ConditionClause(string Condition, params WhereCondition[] ics) : this(Condition)
		{
			foreach ( WhereCondition ic in ics )
			{
				if ( ic != null )
				{
					Add( ic );
				}
			}
		}

        public override bool SubClauseNotEmpty
        {
            get
            {
                foreach (WhereCondition ic in List)
                {
                    if (ic.SubClauseNotEmpty)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Add(WhereCondition ic)
		{
			List.Add( ic );
		}

		public WhereCondition this[int index]
		{
			get { return (WhereCondition)List[index]; }
			set { List[index] = value; }
		}

		public override string ToSqlText(DataParamterCollection dpc, DbDialect dd)
		{
			StringBuilder sb = new StringBuilder();
			foreach ( WhereCondition ic in List )
			{
                if (ic.SubClauseNotEmpty)
                {
                    sb.Append("(");
                    sb.Append(ic.ToSqlText(dpc, dd));
                    sb.Append(") ");
                    sb.Append(Condition);
                    sb.Append(" ");
                }
			}
			string s = sb.ToString();
			return ( s.Length > 5 ) ? s.Substring(0, s.Length - Condition.Length - 2) : "";
		}
	}
}
