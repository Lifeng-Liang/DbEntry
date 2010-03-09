using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lephone.Data.Definition;

namespace Lephone.Data.Linq
{
    public class LinqQueryProvider<T, TKey> : IOrderedQueryable<T>, IQueryProvider where T : DbObjectModelBase<T, TKey>
    {
        private readonly Expression _expression;

        public LinqQueryProvider(Expression expression)
        {
            this._expression = expression;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            var lep = new LinqExpressionParser<T>(this._expression);
            var list = DbEntry.From<T>().Where(lep.Condition).OrderBy(lep.Orderby).Select();
            return ((IEnumerable<T>)list).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // never used ???
            return GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get
            {
                return _expression ?? Expression.Constant(this);
            }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        #endregion

        #region IQueryProvider Members

        public IQueryable<TElement> CreateQuery<TElement>(Expression expr)
        {
            return (IQueryable<TElement>)new LinqQueryProvider<T, TKey>(expr);
        }

        public IQueryable CreateQuery(Expression expr)
        {
            return new LinqQueryProvider<T, TKey>(expr);
        }

        public TResult Execute<TResult>(Expression expr)
        {
            // never used ???
            return (TResult)CreateQuery(expr).GetEnumerator();
        }

        public object Execute(Expression expr)
        {
            // never used ???
            return CreateQuery(expr).GetEnumerator();
        }

        #endregion
    }
}