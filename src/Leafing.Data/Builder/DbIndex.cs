using Leafing.Data.Common;

namespace Leafing.Data.Builder {
    public class DbIndex {
        public string IndexName;
        public ASC[] Columns;
        public bool Unique;

        public DbIndex(string indexName, bool unique, params ASC[] columns) {
            this.IndexName = indexName;
            this.Columns = columns;
            this.Unique = unique;
        }
    }
}