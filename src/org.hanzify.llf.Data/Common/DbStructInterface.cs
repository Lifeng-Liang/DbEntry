
#region usings

using System;

#endregion

namespace Lephone.Data.Common
{
    public class DbStructInterface
    {
        public bool FiltrateDatabaseName;
        public string TablesTypeName;
        public string[] TablesParams;
        public string ViewsTypeName;
        public string[] ViewsParams;
        public string TableNameString;

        public DbStructInterface(string TablesTypeName, string[] TablesGetter, string ViewsTypeName, string[] ViewsGetter, string TableNameString)
            : this(false, TablesTypeName, TablesGetter, ViewsTypeName, ViewsGetter, TableNameString)
        {
        }

        public DbStructInterface(bool FiltrateDatabaseName, string TablesTypeName, string[] TablesGetter, string ViewsTypeName, string[] ViewsGetter, string TableNameString)
        {
            this.FiltrateDatabaseName = FiltrateDatabaseName;
            this.TablesTypeName = (TablesTypeName != null) ? TablesTypeName : "Tables";
            this.TablesParams = (TablesGetter != null) ? TablesGetter : new string[] { };
            this.ViewsTypeName = (ViewsTypeName != null) ? ViewsTypeName : "Views";
            this.ViewsParams = (ViewsGetter != null) ? ViewsGetter : new string[] { };
            this.TableNameString = (TableNameString != null) ? TableNameString : "TABLE_NAME";
        }
    }
}
