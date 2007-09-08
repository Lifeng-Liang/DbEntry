
#region usings

using System;
using Lephone.Data.SqlEntry;

#endregion

namespace Lephone.Data.Builder
{
	public interface ISqlValues
	{
		KeyValueCollection Values { get; }
	}
}
