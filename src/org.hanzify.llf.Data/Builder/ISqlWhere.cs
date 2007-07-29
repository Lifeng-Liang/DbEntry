
#region usings

using System;
using org.hanzify.llf.Data.Builder.Clause;

#endregion

namespace org.hanzify.llf.Data.Builder
{
	public interface ISqlWhere
	{
		WhereClause Where { get; }
	}
}
