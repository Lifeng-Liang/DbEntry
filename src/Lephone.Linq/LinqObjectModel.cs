
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lephone.Data.Definition;
using Lephone.Data.Common;
using System.Linq.Expressions;
using Lephone.Data;

namespace Lephone.Linq
{
    public partial class LinqObjectModel<T, TKey> : DbObjectModel<T, TKey> where T : LinqObjectModel<T, TKey>
    {
        public static LinqQueryProvider<T, TKey> Table
        {
            get { return new LinqQueryProvider<T, TKey>(null); }
        }

        public static DbObjectList<T> Find(Expression<Func<T, bool>> condition)
        {
            return DbEntry.From<T>().Where(condition).Select();
        }

        public static DbObjectList<T> Find(Expression<Func<T, bool>> condition, Expression<Func<T, object>> orderby)
        {
            return DbEntry.From<T>().Where(condition).OrderBy(orderby).Select();
        }

        public static DbObjectList<T> Find(Expression<Func<T, bool>> condition, string orderby)
        {
            return DbEntry.From<T>().Where(condition).OrderBy(orderby).Select();
        }

        public static DbObjectList<T> FindAll(Expression<Func<T, object>> orderby)
        {
            return DbEntry.From<T>().Where(WhereCondition.EmptyCondition).OrderBy(orderby).Select();
        }

        public static T FindOne(Expression<Func<T, bool>> condition)
        {
            return DbEntry.Context.GetObject(condition);
        }
    }

    public class LinqObjectModel<T> : LinqObjectModel<T, long> where T : LinqObjectModel<T, long>
    {
    }
}
