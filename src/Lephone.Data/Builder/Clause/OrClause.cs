using System;
using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.Data.Builder.Clause
{
	[Serializable]
	public class OrClause : ConditionClause
	{
		public OrClause(params Condition[] kvs) : base("OR", kvs)
		{
		}

		public OrClause(string Key, params object[] values) : base("OR")
		{
			foreach ( object o in values )
			{
				Add(new KeyValueClause(Key, o, CompareOpration.Equal, ColumnFunction.None));
			}
		}
	}
}
