
#region usings

using System;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder
{
	public interface ISqlValues
	{
		KeyValueCollection Values { get; }
	}
}
