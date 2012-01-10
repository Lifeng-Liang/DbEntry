using System;
using System.Collections.Generic;
using Leafing.Data.Builder.Clause;

namespace Leafing.Data.Model
{
    public class CrossTable
    {
        public class RefColumn
        {
            public readonly string Table;
            public readonly string Column;

            public RefColumn(string table, string column)
            {
                this.Table = table;
                this.Column = column;
            }
        }

        public readonly Type HandleType;
        public readonly FromClause From;
        public readonly string Name;
        private readonly List<RefColumn> _columns;

        public CrossTable(Type handleType, FromClause from, string name, 
            string table1, string column1, string table2, string column2)
        {
            this.HandleType = handleType;
            this.From = from;
            this.Name = name;
            _columns = new List<RefColumn> {new RefColumn(table1, column1), new RefColumn(table2, column2)};
        }

        public RefColumn this[int n]
        {
            get { return _columns[n]; }
        }

        public List<RefColumn> GetSortedColumns()
        {
            var list = new List<RefColumn>(_columns);
            list.Sort((x, y) => x.Column.CompareTo(y.Column));
            return list;
        }
    }
}

