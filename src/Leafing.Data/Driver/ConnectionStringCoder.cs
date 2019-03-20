using System;
using Leafing.Core.Ioc;

namespace Leafing.Data.Dialect {
    [DependenceEntry, Implementation(1)]
    public class ConnectionStringCoder {
        public virtual string Decode(string source) {
            return source;
        }
    }
}