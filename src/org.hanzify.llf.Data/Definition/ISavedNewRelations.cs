using System;
using System.Collections.Generic;
using System.Text;

namespace Lephone.Data.Definition
{
    public interface IHasAndBelongsToManyRelations
    {
        List<object> SavedNewRelations { get;}
        List<object> RemovedRelations { get;}
    }
}
