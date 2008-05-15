namespace Lephone.Data.Builder
{
    public class DbIndex
    {
        public string IndexName;
        public ASC[] Columns;
        public bool UNIQUE;

        public DbIndex(string IndexName, bool UNIQUE, params ASC[] Columns)
        {
            this.IndexName = IndexName;
            this.Columns = Columns;
            this.UNIQUE = UNIQUE;
        }
    }
}
