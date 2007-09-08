
#region usings

using System;
using Lephone.Util;

#endregion

namespace Lephone.Data.Definition
{
    public interface IBelongsTo : ILazyLoading
    {
        object ForeignKey { get; set; }
        event CallbackObjectHandler<string> ValueChanged;
    }
}
