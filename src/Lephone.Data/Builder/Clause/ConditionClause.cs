using System;
using System.Collections;
using System.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public abstract class ConditionClause : Condition
	{
		private readonly string Condition;
		private readonly ArrayList List = new ArrayList();

	    protected ConditionClause(string Condition)
		{
			this.Condition = Condition;
		}

	    protected ConditionClause(string Condition, params Condition[] ics) : this(Condition)
		{
			foreach ( Condition ic in ics )
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
                foreach (Condition ic in List)
                {
                    if (ic.SubClauseNotEmpty)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Add(Condition ic)
		{
			List.Add( ic );
		}

		public Condition this[int index]
		{
			get { return (Condition)List[index]; }
			set { List[index] = value; }
		}

		public override string ToSqlText(DataParameterCollection dpc, DbDialect dd)
		{
			var sb = new StringBuilder();
			foreach ( Condition ic in List )
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
