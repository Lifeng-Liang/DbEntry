using System;
using System.Collections.Generic;
using System.Text;

namespace org.hanzify.llf.Data.Definition
{
    public interface ISavedNewRelations
    {
        // TODO: Should use object or long ?
        List<long> SavedNewRelations { get;}
    }
}
