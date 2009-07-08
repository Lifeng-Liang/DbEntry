using System.Collections.Generic;

namespace Lephone.Data.Builder
{
    public interface ISqlKeys
    {
        List<KeyValuePair<string, string>> Keys { get; }
    }
}
