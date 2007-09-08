
#region usings

using System;
using Lephone.Data.Builder.Clause;

#endregion

namespace Lephone.Data.Builder
{
	public interface ISqlWhere
	{
		WhereClause Where { get; }
	}
}
