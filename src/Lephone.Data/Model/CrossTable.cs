using System;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data.Model
{
    public class CrossTable
    {
        public Type HandleType;
        public FromClause From;
        public string Name;
        public string ColumeName1;
        public string ColumeName2;

        public CrossTable(Type handleType, FromClause from, string name, string columeName1, string columeName2)
        {
            this.HandleType = handleType;
            this.From = from;
            this.Name = name;
            this.ColumeName1 = columeName1;
            this.ColumeName2 = columeName2;
        }
    }
}

