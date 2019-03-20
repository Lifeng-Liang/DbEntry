using System.Collections.Generic;

namespace Leafing.Data.Definition {
    public interface IHasAndBelongsToManyRelations {
        List<object> SavedNewRelations { get; }
        List<object> RemovedRelations { get; }
    }
}