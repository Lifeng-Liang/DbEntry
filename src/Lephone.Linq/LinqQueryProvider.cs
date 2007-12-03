
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Lephone.Linq
{
    public class LinqQueryable<T, TKey> : IOrderedQueryable<T> where T : LinqObjectModel<T, TKey>
    {
        private Expression exp;
        private IQueryProvider provider;

        public LinqQueryable(Expression exp)
        {
            this.exp = exp;
            provider = new LinqQueryProvider();
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return exp; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }

        #endregion
    }

    public class LinqQueryProvider : IQueryProvider
    {
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
