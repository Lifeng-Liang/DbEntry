using System.Collections.Generic;

namespace Leafing.Data.Builder {
    public interface ISqlKeys {
        List<KeyValuePair<string, string>> Keys { get; }
    }
}
