
#region usings

using System;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
{
    public interface IHasConnection
    {
        ConnectionContext ConnectionProvider { get; }
    }
}
