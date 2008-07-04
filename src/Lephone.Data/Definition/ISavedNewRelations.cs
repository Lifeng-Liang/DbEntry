using System.Collections.Generic;

namespace Lephone.Data.Definition
{
    public interface IHasAndBelongsToManyRelations
    {
        List<object> SavedNewRelations { get;}
        List<object> RemovedRelations { get;}
    }
}
