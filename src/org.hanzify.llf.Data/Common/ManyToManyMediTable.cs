
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data.Common
{
    public class ManyToManyMediTable
    {
        public Type HandleType;
        public FromClause From;
        public string Name;
        public string ColumeName1;
        public string ColumeName2;

        public ManyToManyMediTable(Type HandleType, FromClause From, string Name, string ColumeName1, string ColumeName2)
        {
            this.HandleType = HandleType;
            this.From = From;
            this.Name = Name;
            this.ColumeName1 = ColumeName1;
            this.ColumeName2 = ColumeName2;
        }
    }
}
