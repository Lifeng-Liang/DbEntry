using System.Collections.Generic;
using System.Data;
using Lephone.Data.Builder;

namespace Lephone.Data.Common
{
    public interface IDbObjectHandler
    {
        object CreateInstance();
        void LoadSimpleValues(object o, bool UseIndex, IDataReader dr);
        void LoadRelationValues(DbContext driver, object o, bool UseIndex, IDataReader dr);
        Dictionary<string, object> GetKeyValues(object o);
        object GetKeyValue(object o);
        void SetValuesForSelect(ISqlKeys isv);
        void SetValuesForInsert(ISqlValues isv, object obj);
        void SetValuesForUpdate(ISqlValues isv, object obj);
    }
}
