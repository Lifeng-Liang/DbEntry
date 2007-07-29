
#region usings

using System;
using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    public interface IBelongsTo : ILazyLoading
    {
        object ForeignKey { get; set; }
        event CallbackObjectHandler<string> ValueChanged;
    }
}
