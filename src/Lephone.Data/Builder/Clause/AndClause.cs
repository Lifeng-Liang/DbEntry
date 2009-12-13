using System;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class AndClause : ConditionClause
	{
		public AndClause(params Condition[] kvs) : base("AND", kvs)
		{
		}
	}
}
