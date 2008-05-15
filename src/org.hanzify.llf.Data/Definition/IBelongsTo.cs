using Lephone.Util;

namespace Lephone.Data.Definition
{
    public interface IBelongsTo : ILazyLoading
    {
        object ForeignKey { get; set; }
        event CallbackObjectHandler<string> ValueChanged;
    }
}
