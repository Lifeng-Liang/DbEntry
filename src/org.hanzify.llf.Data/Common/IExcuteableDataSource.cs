
using System;
using System.Collections;
using System.Web.UI;

namespace org.hanzify.llf.Data.Common
{
    public interface IExcuteableDataSource
    {
        IEnumerable ExecuteSelect(DataSourceSelectArguments arguments);
        int ExecuteDelete(IDictionary keys, IDictionary values);
        int ExecuteInsert(IDictionary values);
        int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues);
    }
}
