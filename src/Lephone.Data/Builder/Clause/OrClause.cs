using System;
using Lephone.Data.Definition;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class OrClause : ConditionClause
	{
		public OrClause(params WhereCondition[] kvs) : base("Or", kvs)
		{
		}

		public OrClause(string Key, params object[] values) : base("Or")
		{
			foreach ( object o in values )
			{
				Add(new KeyValueClause(Key, o, CompareOpration.Equal, false));
			}
		}
	}
}
