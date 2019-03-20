using System.Collections.Generic;
using Leafing.Data.Builder.Clause;

namespace Leafing.Data.Builder {
    public interface ISqlValues {
        List<KeyOpValue> Values { get; }
    }
}
