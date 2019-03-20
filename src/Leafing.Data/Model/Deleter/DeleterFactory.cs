using Leafing.Data.Model.Composer;
using Leafing.Data.Model.Handler;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Model.Deleter {
    static class DeleterFactory {
        public static SimpleDeleter CreateDeleter(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler) {
            if (info.HasRelation) {
                return new RelationModelDeleter(info, composer, provider, handler);
            }
            if (info.HasOnePrimaryKey && info.KeyMembers[0].UnsavedValue != null) {
                return new KeyModelDeleter(info, composer, provider, handler);
            }
            return new SimpleDeleter(info, composer, provider, handler);
        }
    }
}