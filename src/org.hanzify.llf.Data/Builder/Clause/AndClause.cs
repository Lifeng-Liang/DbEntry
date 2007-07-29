
using System;

namespace org.hanzify.llf.Data.Builder.Clause
{
	[Serializable]
	public class AndClause : ConditionClause
	{
		public AndClause(params WhereCondition[] kvs) : base("And", kvs)
		{
		}
	}
}
