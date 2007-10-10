
using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lephone.Data;

namespace Lephone.Web.Common
{
    public interface IExcuteableDataSource
    {
        IEnumerable Select(DataSourceSelectArguments arguments);
        int Delete(IDictionary keys, IDictionary values);
        int Insert(IDictionary values);
        int Update(IDictionary keys, IDictionary values, IDictionary oldValues);
    }
}
