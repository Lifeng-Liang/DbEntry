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

        public DbStructInterface(string tablesTypeName, string[] tablesGetter, string viewsTypeName, 
            string[] viewsGetter, string tableNameString)
            : this(false, tablesTypeName, tablesGetter, viewsTypeName, viewsGetter, tableNameString)
        {
        }

        public DbStructInterface(bool filtrateDatabaseName, string tablesTypeName, string[] tablesGetter,
            string viewsTypeName, string[] viewsGetter, string tableNameString)
        {
            this.FiltrateDatabaseName = filtrateDatabaseName;
            this.TablesTypeName = tablesTypeName ?? "Tables";
            this.TablesParams = tablesGetter ?? new string[] { };
            this.ViewsTypeName = viewsTypeName ?? "Views";
            this.ViewsParams = viewsGetter ?? new string[] { };
            this.TableNameString = tableNameString ?? "TABLE_NAME";
        }
    }
}
