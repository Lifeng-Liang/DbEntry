using Leafing.Data.Common;

namespace Leafing.Data.Dialect {
    public class NSQLite : SQLite {
        public override DbStructInterface GetDbStructInterface() {
            return new DbStructInterface("TABLES", null, null, null, "tbl_name");
        }

        public override bool ExecuteEachLine {
            get {
                return true;
            }
        }
    }
}