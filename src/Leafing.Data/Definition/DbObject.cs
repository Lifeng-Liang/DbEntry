using System;

namespace Leafing.Data.Definition {
    [Serializable]
    public abstract class DbObject : DbObjectBase {
        [DbKey]
        public long Id { get; set; }

        protected DbObject() {
        }
    }
}
