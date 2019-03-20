using Leafing.Data.Definition;
using Leafing.Data.Model.Composer;
using Leafing.Data.Model.Handler;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Model.Saver {
    static class SaverFactory {
        public static SimpleObjectSaver CreateSaver(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler) {
            if (info.HasSystemKey && info.KeyMembers[0].UnsavedValue == null) {
                throw new DataException("System key must have UnsavedValue.");
            }

            if (info.HandleType.IsSubclassOf(typeof(DbObjectSmartUpdate))) {
                if (info.HasRelation) {
                    return new RelationModelSaver(info, composer, provider, handler);
                }
                return new DbModelSaver(info, composer, provider, handler);
            }
            if (info.HasSystemKey) {
                return new DbObjectSaver(info, composer, provider, handler);

            }
            return new SimpleObjectSaver(info, composer, provider, handler);
        }
    }
}