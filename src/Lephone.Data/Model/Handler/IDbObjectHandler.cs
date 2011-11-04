using System.Collections.Generic;
using System.Data;
using Lephone.Data.Builder;

namespace Lephone.Data.Model.Handler
{
    public interface IDbObjectHandler
    {
        object CreateInstance();
        void LoadSimpleValues(object o, bool useIndex, IDataReader dr);
        void LoadRelationValues(object o, bool useIndex, bool noLazy, IDataReader dr);
        Dictionary<string, object> GetKeyValues(object o);
        object GetKeyValue(object o);
        void SetKeyValue(object obj, object key);
        void SetValuesForSelect(ISqlKeys isv, bool noLazy);
        void SetValuesForInsert(ISqlValues isv, object obj);
        void SetValuesForUpdate(ISqlValues isv, object obj);
    }
}
